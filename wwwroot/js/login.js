function validateLoginInput()
{
    let loginForm = document.getElementById('loginForm');
    if (loginForm.username.value.length < 1 || loginForm.password.value.length < 1)
    {
        return false;
    }

    return true;
}


function loginSubmit()
{
    let loginForm = document.getElementById('loginForm');

    data = {
        username: loginForm.username.value,
        firstname: '',
        lastname: '',
        email: '',
        password: loginForm.password.value
    };


    if (!validateLoginInput())
        return false;


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
            loginInSuccessful();
        }
        else
        {
            loginInUnsuccessful();
        }
    });
}


function loginInSuccessful()
{
    window.location.href = "/";
}


function loginInUnsuccessful()
{
    let loginForm = document.getElementById('loginForm');
    loginForm.username.value = '';
    loginForm.password.value = '';
    alert('Niepoprawne dane logowania!');
}


function validateRegisterInput()
{
    let registerForm = document.getElementById('registerForm');
    let valid = true;
    let info = document.getElementById('error');
    info.innerHTML = "";        
    info.style.color = 'red';


    if (registerForm.username.value.length < 1 || registerForm.firstname.value.length < 1
        || registerForm.lastname.value.length < 1 || registerForm.password.value.length < 1 )
    {
        info.innerHTML += "Wszystkie pola muszą być wypełnione.\n";    
        valid = false;
    }


    if (!registerForm.email.value.match(".*@.*") || registerForm.email.value.length < 3)
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
    let registerForm = document.getElementById('registerForm');
    data = {
        username: registerForm.username.value,
        firstname: registerForm.firstname.value,
        lastname: registerForm.lastname.value,
        email: registerForm.email.value,
        password: registerForm.password.value
    };


    if (!validateRegisterInput())
        return false;

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
            registrationUnsuccessful();
        }
        else
        {
            registrationSuccessful();
        }
    });
}


function registrationSuccessful()
{
    let registerForm = document.getElementById('registerForm');
    document.getElementById('registerFormUsernameLabel').innerHTML = "Nazwa użytkownika";
    document.getElementById('registerFormUsername').style.border = "0";
    registerForm.username.value = '';
    registerForm.firstname.value = '';
    registerForm.lastname.value = '';
    registerForm.email.value = '';
    registerForm.password.value = '';
    let info = document.getElementById('error');
    info.style.color = 'green';
    info.innerHTML = "Rejestracja powiodła się!";
}


function registrationUnsuccessful()
{
    let registerForm = document.getElementById('registerForm').username.value = '';
    document.getElementById('registerFormUsername').style.border = "solid 1px red";
    document.getElementById('registerFormUsernameLabel').innerHTML = "Nazwa użytkownika (podana jest zajęta)";
}


function loginFormEnterPressed(e)
{
    if (e.keyCode == 13)
        loginSubmit();
}


function onLoad()
{
    document.getElementById('loginFormUsername').addEventListener('keypress', loginFormEnterPressed);
    document.getElementById('loginFormPassword').addEventListener('keypress', loginFormEnterPressed);
}


document.addEventListener('DOMContentLoaded', onLoad);