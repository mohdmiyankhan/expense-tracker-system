using ExpenseTrackerSystem.Data;
using ExpenseTrackerSystem.Helpers;
using ExpenseTrackerSystem.Models;
using ExpenseTrackerSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTrackerSystem.Controllers
{
    public class ExpenseController : Controller
    {
        private ExpenseDbContext db = new ExpenseDbContext();

        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            ExpenseVM vm = new ExpenseVM();
            vm.ExpenseDate = DateTime.Now;

            vm.Users = db.Users
                .Select(x => new SelectListItem
                {
                    Value = x.UserId.ToString(),
                    Text = x.Name
                }).ToList();

            vm.Groups = db.UserGroups
                .Select(x => new SelectListItem
                {
                    Value = x.GroupId.ToString(),
                    Text = x.GroupName
                }).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpenseVM model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Users = db.Users
                    .Select(x => new SelectListItem
                    {
                        Value = x.UserId.ToString(),
                        Text = x.Name
                    }).ToList();

                model.Groups = db.UserGroups
                    .Select(x => new SelectListItem
                    {
                        Value = x.GroupId.ToString(),
                        Text = x.GroupName
                    }).ToList();

                return View(model);
            }

            int currentUserId = UserHelper.GetCurrentUserId();

            model.PaidById = currentUserId;

            Expense expense = new Expense()
            {
                Title = model.Title,
                Amount = model.Amount,
                PaidById = model.PaidById,
                GroupId = model.GroupId,
                ExpenseDate = model.ExpenseDate
            };

            db.Expenses.Add(expense);
            db.SaveChanges();

            // Get selected group members
            var group = db.UserGroups.Find(model.GroupId);

            var memberIds = group.Members.Split(',').Select(int.Parse).ToList();

            decimal share = model.Amount / memberIds.Count;

            foreach (var id in memberIds)
            {
                ExpenseShare s = new ExpenseShare()
                {
                    ExpenseId = expense.ExpenseId,
                    UserId = id,
                    ShareAmount = share
                };

                db.ExpenseShares.Add(s);
            }

            db.SaveChanges();

            TempData["SuccessMsg"] = "Expense Saved Successfully";

            return RedirectToAction("Create");
        }

        public ActionResult History(int groupId = 0)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = UserHelper.GetCurrentUserId();

            // Get All Groups
            var groups = db.UserGroups.ToList();
            ViewBag.Groups = groups;

            if (groupId == 0)
                groupId = currentUserId;

            ViewBag.GroupId = groupId;

            // Selected Group
            var selectedGroup = groups.FirstOrDefault(x => x.GroupId == groupId);

            // Group Members
            var memberIds = selectedGroup.Members.Split(',').Select(x => Convert.ToInt32(x.Trim())).ToList();

            // Get Only Group Users
            var users = db.Users.Where(x => x.IsActive == 1 && memberIds.Contains(x.UserId)).ToList();

            var groupIds = db.UserGroupMembers.Where(x => x.UserId == currentUserId).Select(x => x.GroupId).ToList();

            var expenseList = db.Expenses.ToList();
            var expenses = expenseList.Where(x => x.GroupId == groupId && memberIds.Contains(x.PaidById))
                .OrderByDescending(x => x.ExpenseDate).ToList();

            //var expenses = db.Expenses
            //    .Include("PaidBy")
            //    .Include("Group")
            //    .Where(x => groupIds.Contains(x.GroupId))
            //    .OrderByDescending(x => x.ExpenseDate)
            //    .ToList();

            return View(expenses);
        }

        public ActionResult Edit(int Id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = UserHelper.GetCurrentUserId();

            var expense = db.Expenses.Find(Id);
            if (expense == null)
            {
                TempData["ErrorMsg"] = "Data not found";
                return RedirectToAction("History");
            }

            if (expense.PaidById != currentUserId)
            {
                TempData["ErrorMsg"] = "You are not allowed to edit this expense";
                return RedirectToAction("History");
            }

            ExpenseVM vm = new ExpenseVM();

            vm.Title = expense.Title;
            vm.Amount = Math.Round(expense.Amount);
            vm.PaidById = expense.PaidById;
            vm.GroupId = expense.GroupId;
            vm.ExpenseDate = expense.ExpenseDate;

            vm.Users = db.Users.Select(x => new SelectListItem
            {
                Value = x.UserId.ToString(),
                Text = x.Name
            }).ToList();

            vm.Groups = db.UserGroups.Select(x => new SelectListItem
            {
                Value = x.GroupId.ToString(),
                Text = x.GroupName
            }).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, ExpenseVM model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var exp = db.Expenses.Find(Id);

            exp.Title = model.Title;
            exp.Amount = model.Amount;
            exp.PaidById = model.PaidById;
            exp.GroupId = model.GroupId;
            exp.ExpenseDate = model.ExpenseDate;

            db.SaveChanges();

            // Remove old shares
            var oldShares = db.ExpenseShares.Where(x => x.ExpenseId == Id).ToList();

            db.ExpenseShares.RemoveRange(oldShares);
            db.SaveChanges();

            var members = db.UserGroups.Find(model.GroupId).Members.Split(',').Select(int.Parse).ToList();

            decimal share = model.Amount / members.Count;

            foreach (var user in members)
            {
                db.ExpenseShares.Add(new ExpenseShare
                {
                    ExpenseId = Id,
                    UserId = user,
                    ShareAmount = share
                });
            }

            db.SaveChanges();

            TempData["SuccessMsg"] = "Expense Updated Successfully";

            return RedirectToAction("History");
        }

        public ActionResult Settle(string data = "")
        {
            int fromUserId = 0, toUserId = 0, amount = 0;

            ViewBag.SettlementUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Report/Settle?data=" + data;

            if (!string.IsNullOrEmpty(data))
            {
                string decryptedData = UrlEncryptionHelper.Decrypt(data);
                var query = HttpUtility.ParseQueryString(decryptedData);
                fromUserId = Convert.ToInt32(query["fromUserId"]);
                toUserId = Convert.ToInt32(query["toUserId"]);
                amount = Convert.ToInt32(query["amount"]);
            }

            var users = db.Users.ToList();
            var toUser = users.Where(x => x.UserId == toUserId).FirstOrDefault();

            string upiId = toUser.UPIId;
            string name = toUser.Name;
            string amountToPay = Convert.ToString(amount);
            string txnId = "T" + DateTime.Now.ToString("yyMMddHHmmssfff").ToUpper();
            string note = "Settlement of ₹ " + amountToPay + " pay to " + name;

            string upiUrl = $"upi://pay?pa={Uri.EscapeDataString(upiId)}" +
                            $"&pn={Uri.EscapeDataString(name)}" +
                            $"&am={amountToPay}" +
                            $"&cu=INR" +
                            $"&tr={txnId}" +
                            $"&tn={Uri.EscapeDataString(note)}";

            ViewBag.PayNowUrl = upiUrl;

            ViewBag.Amount = amount;

            return View();
        }
    }
}