using ExpenseTrackerSystem.Data;
using ExpenseTrackerSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTrackerSystem.Controllers
{
    public class HomeController : Controller
    {
        private ExpenseDbContext db = new ExpenseDbContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = UserHelper.GetCurrentUserId();

            var expenses = db.Expenses.ToList();

            ViewBag.TotalExpense = Math.Round(expenses.Where(x => x.PaidById == currentUserId).Sum(x => (decimal?)x.Amount) ?? 0);

            ViewBag.TotalEntry = expenses.Where(x => x.PaidById == currentUserId).Count();

            ViewBag.TotalSettlement = db.Settlements.Where(x => x.FromUserId == currentUserId).ToList().Count();

            ViewBag.User = db.Users.Count();

            var recent = db.Expenses.Include("PaidBy").Where(x => x.PaidById == currentUserId).OrderByDescending(x => x.ExpenseDate).Take(25).ToList();

            return View(recent);
        }
    }
}