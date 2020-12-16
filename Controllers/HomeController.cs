﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LogReg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace LogReg.Controllers
{
    public class HomeController : Controller
    {
        private HomeContext _context {get;set;}

        public HomeController(HomeContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

    [HttpPost("register")]
    public IActionResult Register(User NewUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(u => u.Email == NewUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
                
                return View("Index",NewUser);
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
            _context.Users.Add(NewUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("LoggedInID",(int)NewUser.UserId);
            return RedirectToAction("Success");
        }
        else
        {
            return View("Index",NewUser);
        }
    } 

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost("login")]
        public IActionResult Process(LoginUser ThisUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == ThisUser.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LogIn", "Invalid Credentials.");
                    return View("Login");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(ThisUser, userInDb.Password, ThisUser.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LogIn", "Invalid Credentials.");
                    return View("login");
                }
                HttpContext.Session.SetInt32("LoggedInID", (int)userInDb.UserId);
                return RedirectToAction("Success");
            }
            return View("login");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            
            var isInSession = HttpContext.Session.GetInt32("LoggedInID");
            if(isInSession > 0)
            {
                return View("Success");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
