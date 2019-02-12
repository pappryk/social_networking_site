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
public class HomeController : Controller
{
    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {

        return View();
    }

    [HttpGet]
    [Route("/login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (HttpContext.User.Identity.IsAuthenticated)
            return Redirect("/index");
        return View();
    }

}