/////////////////////////////////////
// Please, don't look at this file //
/////////////////////////////////////


function searchGroups(e)
{
    if (e.keyCode == 13)
    {
        e.preventDefault();
        fetch("group/search/" + e.target.value)
            .then(response => response.json())
            .then(data => {
                let main = document.getElementById('main');
                let val = "Znalezione grupy:";
                if (data.length < 1)
                    val = "Brak wyników dla \"" + e.target.value + "\"";
                let toRender = "<div class='friend-list'><h3>" + val + "</h3>";
                main.innerHTML = toRender + displayGroups(data) + "</div>";
            });
    }
    else
    {
        return false;
    }
}



function fetchGroups(e = null)
{
    let username = e == null ? getCookie('username') : e.target.getAttribute('username');
    let main = document.getElementById('main');
    fetch("group/get/" + username)
        .then(response => response.json())
        .then(data => {
            let toRender = "";
            for (let l = 0; l < data.length; ++l)
            {
                toRender += "<p>" + data[l].name + "</p>";
            }

            main.innerHTML = toRender;
        });
}



function displayGroups(groups)
{
    let toRender = "";
    for (let w = 0; w < groups.length; ++w)
    {
        toRender += "<a groupname='" + groups[w].name + "' class='groupInfoLink' href='#'>" + groups[w].name + "</a><label>";
    }
    
    return toRender;
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



async function onLoad()
{

    let myModal = document.getElementById('modal-window');

    window.onclick = function(event) {
        if (event.target == myModal) {
            myModal.style.display = "none";
        }
    }

    document.getElementById('search-bar').addEventListener('keydown', searchGroups);


    // console.log(posts);
}


document.addEventListener('DOMContentLoaded', onLoad);