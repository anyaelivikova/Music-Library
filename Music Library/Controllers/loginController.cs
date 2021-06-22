using Music_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Music_Library.Controllers
{

    public class loginController : Controller
    {
        private readonly Entities _dbContext = new Entities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(users user)
        {
            if (ModelState.IsValid)
            {
                bool IsValidUser = _dbContext.users
               .Any(u => u.user_name.ToLower() == user
               .user_name.ToLower() && u.user_password == user.user_password);

                if (IsValidUser)
                {
                    FormsAuthentication.SetAuthCookie(user.user_name, false);
                    
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "invalid Username or Password");
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(users registerUser)
        {
            if (ModelState.IsValid)
            {
                _dbContext.users.Add(registerUser);
                _dbContext.SaveChanges();
                return RedirectToAction("Login");

            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}