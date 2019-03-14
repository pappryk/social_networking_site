class Post {
    constructor(data) {
        this.data = data;
    }
    
    
    assignComments(comments) {
        this.comments = comments;
    }

    render() {
        let toRender = "<div postid='" + this.data.id + "' class='post'><p style='font-size: 20px'><a username='" + this.data.username + "' class='userInfoLink' href='#'>" + this.data.username + "</a></p>";
        toRender += "<p style='font-size: 10px'>" + this.data.datePublished.replace('T', ' ').substring(0, 19) + "</p>";
        toRender += this.data.text;
        toRender += "<br><br><hr><button class='likeButton' postid='" + this.data.id + "'><img src='/img/likebtn.png'/></button>";
        toRender += "<button class='likesCounterButton' style='font-size: 18px;'>" + this.data.likesCounter + "</button>";
        toRender += "<br><br><textarea class='comment-textarea' placeholder='Komentarz'></textarea>";
        
        for (let k = 0; k < this.comments.length; k++)
        {
            toRender += "<br><br><div class='comment-field'> <a username='" + this.comments[k].username + "' class='userInfoLink' href='#'>" + this.comments[k].username + "</a>" + this.comments[k].text;
            toRender += "<br><div style='font-size:12px'>" + this.comments[k].datePublished.replace('T', ' ').substring(0, 19) + "</div></div>";
        }

        toRender += "</div>";

        return toRender;
    }
}


/////////////////////////////////////////////////
let maxAmmountOfPosts = 30;
let likeButtons = [];
let posts;
/////////////////////////////////////////////////



function likePost(e)
{
    let data = {
        username: getCookie('username'),
        id: e.target.getAttribute('postid')
    }


    fetch('/user/likepost', {
        method: "POST",
        body: JSON.stringify(data),
        headers : {
            "Content-Type": "application/json"
        }
    })
    .then(response =>
        {
            fetchPosts();
        });
}





function sendNewPost()
{
    let data = {
        text: document.getElementById('newposttextarea').value,
        username: getCookie('username')
    };

    if (data.text.length < 1)
        return false;

    fetch('user/newpost', {
        method: "POST",
        body: JSON.stringify(data),
        headers : {
            "Content-Type": "application/json"
        }
    })
    .then(response => 
        {
            if (response.ok)
            {
                fetchPosts();
            }
            else
                alert("Dodawanie wpisu nie powiodlo sie");

        })
}





async function fetchPosts(username)
{
    let main = document.getElementById('main');

    posts = await (await fetch("user/poststodisplay/" + getCookie('username'))).json();

    main.innerHTML = "<div class='new-post-field'><textarea id='newposttextarea' placeholder='Napisz coś'></textarea><button onclick='sendNewPost()'>Opublikuj</button></div>";

    for (i = 0; i < posts.length; ++i)
    {
        posts[i] = new Post(posts[i]);
        let comments = await fetch("user/comments/" + posts[i].data.id).then(response => response.json());
        posts[i].assignComments(comments);
    }

    for (i = 0; i < posts.length; ++i)
    {
        main.innerHTML += posts[i].render();
    }

    setEventListeners();
}


function sendNewComment(e)
{
    if (e.keyCode == 13 && !e.shiftKey)
    {
        e.preventDefault();
        if (e.target.value.length < 1)
            return false;

        data = {
            text: e.target.value,
            username: getCookie('username'),
            postid: e.target.parentElement.getAttribute('postid')
        }

        fetch('/user/newcomment', {
            method: "POST",
            body: JSON.stringify(data),
            headers : {
                "Content-Type": "application/json"
            }
        })
        .then(response => {
            if (response.ok)
            {
                fetchPosts();
                e.target.value = "";
            }
            else
            {
                alert("Nie udało się wysłać komentarza");
            }
        });
    }
    
        
}



function fetchLikingUsers(e)
{
    let id = e.target.parentElement.getAttribute('postid');

    fetch('user/likersofpost/' + id)
    .then(response => response.json())
    .then(data => {
        let toReturn = "<div class='modal-content'><h3>Użytkownicy lubiący wpis:</h3>";
        for (i = 0; i < data.length; ++i)
        {
            toReturn += "<p>" + data[i].username + "</p>\n";
        }
        toReturn += "</div>";

        let modal = document.getElementById('modal-window');
        modal.innerHTML = toReturn;
        modal.style.display = 'block';
        

        return data;
    });
}






function fetchFriends()
{
    fetch("user/friends/" + getCookie('username'))
        .then(response => response.json())
        .then(data => {
            let main = document.getElementById('main');
            let toRender = "<div class='friend-list'><h3>Znajomi:</h3>";
            main.innerHTML = toRender + displayUsers(data) + "</div>";
            setEventListeners();
        });
}






function displayUsers(users)
{
    let toRender = "";
    for (let w = 0; w < users.length; ++w)
    {
        toRender += "<a username='" + users[w].username + "' class='userInfoLink' href='#'>" + users[w].username + "</a><label>";
        toRender += users[w].firstName + "</label><label> " + users[w].lastName + "<label><br>";
    }
    
    return toRender;
}




function searchUsers(e)
{
    if (e.keyCode == 13 && !e.shiftKey)
    {
        e.preventDefault();
        fetch("user/search/" + e.target.value)
            .then(response => response.json())
            .then(data => {
                let main = document.getElementById('main');
                let val = "Znalezieni użytkownicy:";
                if (data.length < 1)
                    val = "Brak wyników dla \"" + e.target.value + "\"";
                let toRender = "<div class='friend-list'><h3>" + val + "</h3>";
                main.innerHTML = toRender + displayUsers(data) + "</div>";
                
                setEventListeners();
            });
    }
    else
    {
        return false;
    }
}




function fetchUser(e = null)
{
    let username = e == null ? getCookie('username') : e.target.getAttribute('username');
    let main = document.getElementById('main');
    fetch("user/get/" + username)
        .then(response => response.json())
        .then(data => {
            let toRender = "<div username='" + data.username + "' class='user-info'><h1>" + data.username + "</h1>";
            toRender += "Imię: " + data.firstName + "<br>";
            toRender += "Nazwisko: " + data.lastName + "<br>";
            toRender += "Data dołączenia: " + data.dateJoined.substring(0, 10) + "<br>";
            if (data.username != getCookie('username'))
                toRender += "<button id='toggleFriend' class='btn'>Dodaj/usuń znajomego</button>";
            toRender += "</div>";
            main.innerHTML = toRender;

            document.getElementById('toggleFriend').addEventListener('click', addFriend);
        });
}



function addFriend(e)
{
    data = {
        firstName: getCookie('username'),
        lastName: e.target.parentElement.getAttribute('username')
    };

    fetch('/user/newfriend', {
        method: "POST",
        body: JSON.stringify(data),
        headers : {
            "Content-Type": "application/json"
        }
    })
    .then(response => {
        if (!response.ok)
            alert("Nie udało się dodać znajomego!");
    });
}




function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }



function setEventListeners()
{
    likeButtons = document.getElementsByClassName('likeButton');
    for (i = 0; i < likeButtons.length; ++i)
    {
        likeButtons[i].addEventListener('click', likePost);
    }

    
    
    let likesCounterButtons = document.getElementsByClassName('likesCounterButton');
    for (i = 0; i < likesCounterButtons.length; ++i)
    {
        likesCounterButtons[i].addEventListener('click', fetchLikingUsers);
    }


    let commentTextareas = document.getElementsByClassName('comment-textarea');
    for (let w = 0; w < commentTextareas.length; ++w)
    {
        commentTextareas[w].addEventListener('keydown', sendNewComment);
    }


    let userLinks = document.getElementsByClassName('userInfoLink');
    for (let w = 0; w < userLinks.length; ++w)
    {
        userLinks[w].addEventListener('click', fetchUser);
    }

}


async function onLoad()
{
    await fetchPosts();
    let myModal = document.getElementById('modal-window');

    window.onclick = function(event) {
        if (event.target == myModal) {
            myModal.style.display = "none";
        }
    }

    document.getElementById('search-bar').addEventListener('keydown', searchUsers);
}





document.addEventListener('DOMContentLoaded', onLoad);