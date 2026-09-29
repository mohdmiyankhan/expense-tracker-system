using ExpenseTrackerSystem.Data;
using ExpenseTrackerSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ExpenseTrackerSystem.Controllers
{
    public class AccountController : Controller
    {
        private ExpenseDbContext db = new ExpenseDbContext();

        public ActionResult Login()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = db.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password && x.IsActive == 1);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            FormsAuthentication.SetAuthCookie(user.UserId.ToString(), false);

            Session["UserId"] = user.UserId;
            Session["UserName"] = user.Name;

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Logout()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}