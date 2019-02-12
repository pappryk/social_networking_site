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


[Authorize]
[Route("[controller]")]
public class GroupController : Controller
{

    private IConfiguration _configuration;
    private string _connectionString;


    public GroupController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("localhost");
    }


    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }


    [HttpGet("get/{username}")]
    public IActionResult UsersGroups(string username)
    {
        string queryString = $"SELECT * FROM grupy_uzytkownika({username});";
            User user = null; 
            

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(queryString);
                cmd.Connection = con;
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                    
            }

            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return new JsonResult(user);
    }
}