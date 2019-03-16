using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace app.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private IUserService _userService;
        private string _connectionString;

        public UserController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _connectionString = _configuration.GetConnectionString("azure");
            _userService.ConnectionString = _connectionString;
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<User> users = _userService.GetAll();
                        
            return new JsonResult(users);
        }



        [HttpGet]
        [Route("get/{username}")]
        public IActionResult GetUser(string username)
        {
            User user = _userService.GetUser(username);
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return new JsonResult(user);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] User user)
        {
            if (!_userService.IsUserRegistered(user.Username))
            {
                return StatusCode(500);
            }

            _userService.RegisterUser(user);

            Response.Headers.Add("Access-Control-Allow-Origin", "*");            

            return new JsonResult(user);
        }







        ///////////////////////////////////////////////////////
        // The rest not refactored yet                       //
        // And should be create another controller for Posts //
        ///////////////////////////////////////////////////////







        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] User userToLogin)
        {
            User user = null;
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM uzytkownicy WHERE nazwa_uzytkownika = '{userToLogin.Username}';";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                if (!reader.HasRows)
                {
                    return BadRequest("Niepoprawna nazwa użytkownika lub hasło.");
                }

                user = new User(reader);
            }


            PasswordHasher<User> ph = new PasswordHasher<User>();

            if (ph.VerifyHashedPassword(user, user.Password, userToLogin.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Niepoprawna nazwa użytkownika lub hasło.");
            }


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, "placeholder"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("FirstName", user.FirstName)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProps = new AuthenticationProperties(){
                //set properties
                //np: 
                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProps);
            // Convert.ToBase64String()

            Response.Cookies.Append("username", user.Username);

            return Ok(new JsonResult(new string[] {ph.HashPassword(user, userToLogin.Password)}));
        }


        [HttpGet("poststodisplay/{username}")]
        public IActionResult PostsToSee([FromRoute] string username)
        {

            
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM posty_do_wyswietlenia('{username}');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<Post> posts = new List<Post>();

                while (reader.Read())
                {
                    posts.Add(new Post(reader));
                }
                return new JsonResult(posts);
            }

        }


        [HttpGet]
        [Route("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("login", "home");
        }

        [HttpPost("newpost")]
        public IActionResult NewPost([FromBody] Post post)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {

                string queryString = $"INSERT INTO obiekty(tekst, nazwa_uzytkownika, typ_obiektu_id, liczba_polubien) VALUES ('{post.Text}', '{post.Username}', '1', '0');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Headers.Add("Access-Control-Allow-Origin", "*"); 

            return Ok(); 
        }


        [HttpPost("likepost")]
        public IActionResult LikePost(LikePost likePost)
        {
            string queryString = $"SELECT * FROM polubienie_obiekt WHERE nazwa_uzytkownika = '{likePost.Username}' AND obiekt_id = '{likePost.Id}';";

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {

                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    queryString = $"INSERT INTO polubienie_obiekt(nazwa_uzytkownika, obiekt_id) VALUES ('{likePost.Username}', '{likePost.Id}');";
                }
                else
                {
                    queryString = $"DELETE FROM polubienie_obiekt WHERE nazwa_uzytkownika = '{likePost.Username}' AND obiekt_id =  '{likePost.Id}';";
                }

            }


            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {

                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok();
        }


        [HttpGet("likersofpost/{id}")]
        public IActionResult GetLikersOfPost([FromRoute]int id)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM uzytkownicy_lubiacy_wpis('{id}');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<User> users = new List<User>();

                while (reader.Read())
                {
                    users.Add(new User()
                    {
                        Username = (string)reader[0]
                    });
                }
                Response.Headers.Add("Access-Control-Allow-Origin", "*"); 


                return new JsonResult(users);
            }
        }


        [HttpGet("comments/{id}")]
        public IActionResult GetPostComments([FromRoute]int id)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM komentarze WHERE obiekt_id = '{id}';";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<Comment> comments = new List<Comment>();

                while (reader.Read())
                {
                    comments.Add(new Comment(reader));
                }
                Response.Headers.Add("Access-Control-Allow-Origin", "*"); 


                return new JsonResult(comments);
            }
        }


        [HttpPost("newcomment")]
        public IActionResult NewComment([FromBody]Comment comment)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"INSERT INTO komentarze (nazwa_uzytkownika, tekst, obiekt_id, liczba_polubien) VALUES ('{comment.Username}', '{comment.Text}', '{comment.PostId}', '0');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();


                Response.Headers.Add("Access-Control-Allow-Origin", "*"); 

                return Ok();
            }
        }



        [HttpGet("friends/{username}")]
        public IActionResult GetFriends(string username)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM znajomi_uzytkownika('{username}');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<User> users = new List<User>();

                while (reader.Read())
                {
                    users.Add(new User()
                    {
                        Username = (string)reader[0],
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        DateJoined = reader.GetDateTime(3)
                    });
                }


                Response.Headers.Add("Access-Control-Allow-Origin", "*"); 

                return new JsonResult(users);
            }
        }


        [HttpGet("search/{input}")]
        public IActionResult FindUser(string input)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string queryString = $"SELECT * FROM znajdz_uzytkownika('{input}');";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<User> users = new List<User>();

                while (reader.Read())
                {
                    users.Add(new User()
                    {
                        Username = (string)reader[0],
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        DateJoined = reader.GetDateTime(3)
                    });
                }


                Response.Headers.Add("Access-Control-Allow-Origin", "*"); 

                return new JsonResult(users);
            }
        }



        [HttpPost("newfriend")]
        public IActionResult AddFriend([FromBody] User users)
        {
            string queryString = $"select * from znajomi where nazwa_uzytkownika='{users.FirstName}' AND nazwa_znajomego = '{users.LastName}';";
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    queryString = $"SELECT * FROM usun_znajomych('{users.FirstName}', '{users.LastName}');";
                else
                    queryString = $"SELECT * FROM dodaj_znajomych('{users.FirstName}', '{users.LastName}');";

            }

System.Console.WriteLine(queryString + "\n\n\n\n");
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();

            }
            return Ok();
        }
    }
}
