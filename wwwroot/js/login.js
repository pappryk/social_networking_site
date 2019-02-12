function validateLoginInput()
{
    let lf = document.getElementById('loginForm');
    if (lf.username.value.length < 1 || lf.password.value.length < 1)
    {
        return false;
    }

    return true;
}


function loginSubmit()
{
    let lf = document.getElementById('loginForm');

    data = {
        username: lf.username.value,
        firstname: '',
        lastname: '',
        email: '',
        password: lf.password.value
    };


    if (!validateLoginInput())
        return false;


    let resetValues = false;

    fetch('user/login', {
        method: "POST",
        body: JSON.stringify(data),
        headers : {
            "Content-Type": "application/json"
        }
    })
    .then(function(response) {
        
        if (response.ok)
        {
            window.location.href = "/";
        }
        else
        {
            alert('Niepoprawne dane logowania!');
        }


        lf.username.value = '';
        lf.password.value = '';

    });
}


function validateRegisterInput()
{
    let rf = document.getElementById('registerForm');
    let valid = true;
    let info = document.getElementById('error');
    info.innerHTML = "";        
    info.style.color = 'red';


    if (rf.username.value.length < 1 || rf.firstname.value.length < 1
        || rf.lastname.value.length < 1 || rf.password.value.length < 1 )
    {
        info.innerHTML += "Wszystkie pola muszą być wypełnione.\n";    
        valid = false;
    }


    if (!rf.email.value.match(".*@.*") || rf.email.value.length < 3)
    {
        info.innerHTML += "Niepoprawny format e-mail.";
        valid = false;
    }

    if (valid)
    {
        info.innerHTML = "";
        return true;
    }
    return false;
}



function registerSubmit() {
    let rf = document.getElementById('registerForm');
    data = {
        username: rf.username.value,
        firstname: rf.firstname.value,
        lastname: rf.lastname.value,
        email: rf.email.value,
        password: rf.password.value
    };


    if (!validateRegisterInput())
        return false;


    let resetValues = false;

    fetch('user/register', {
        method: "POST",
        body: JSON.stringify(data),
        headers : {
            "Content-Type": "application/json"
        }
    })
    .then(function(response) {
        
        if (!response.ok)
        {
            rf.username.value = '';
            document.getElementById('registerFormUsername').style.border = "solid 1px red";
            document.getElementById('registerFormUsernameLabel').innerHTML = "Nazwa użytkownika (podana jest zajęta)";
        }
        else
        {
            document.getElementById('registerFormUsernameLabel').innerHTML = "Nazwa użytkownika";
            document.getElementById('registerFormUsername').style.border = "0";
            rf.username.value = '';
            rf.firstname.value = '';
            rf.lastname.value = '';
            rf.email.value = '';
            rf.password.value = '';
            let info = document.getElementById('error');
            info.style.color = 'green';
            info.innerHTML = "Rejestracja powiodła się!";
        }
    });

    
}





function enterPressed(e)
{
    if (e.keyCode == 13)
        loginSubmit();
}




function onLoad()
{
    document.getElementById('loginFormUsername').addEventListener('keypress', enterPressed);
    document.getElementById('loginFormPassword').addEventListener('keypress', enterPressed);
}


document.addEventListener('DOMContentLoaded', onLoad);
