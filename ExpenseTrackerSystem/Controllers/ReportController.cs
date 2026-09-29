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
    public class ReportController : Controller
    {
        private readonly ExpenseDbContext db;

        public ReportController()
        {
            db = new ExpenseDbContext();
        }

        public ActionResult Index(int groupId = 0)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = UserHelper.GetCurrentUserId();

            // Get All Groups
            var groups = db.UserGroups.ToList();
            ViewBag.Groups = groups;

            // Agar group select nahi kiya hai
            // to All Group (GroupId = 1) select hoga
            if (groupId == 0)
                groupId = 10;

            ViewBag.GroupId = groupId;

            // Selected Group
            var selectedGroup = groups.FirstOrDefault(x => x.GroupId == groupId);

            // Group Members
            var memberIds = selectedGroup.Members.Split(',').Select(x => Convert.ToInt32(x.Trim())).ToList();

            // Get Only Group Users
            var users = db.Users.Where(x => x.IsActive == 1 && memberIds.Contains(x.UserId)).ToList();

            var expenseList = db.Expenses.ToList();

            // Get Expenses
            // PaidBy user group ka member hona chahiye
            // Expense ko GroupId se filter karo
            var expenses = expenseList.Where(x => x.GroupId == groupId && memberIds.Contains(x.PaidById)).ToList();

            // Get Expense Shares
            var expenseIds = expenses.Select(x => x.ExpenseId).ToList();
            var shares = db.ExpenseShares.Where(x => expenseIds.Contains(x.ExpenseId) && memberIds.Contains(x.UserId)).ToList();

            // Get Settlements
            // FromUser aur ToUser dono same group me hone chahiye.
            var settlements = new List<Settlement>();

            if (expenses.Any())
            {
                settlements = db.Settlements.Where(x => x.IsSettled && memberIds.Contains(x.FromUserId) && memberIds.Contains(x.ToUserId)).ToList();
            }

            // Calculate User Balance
            var userBalances = new List<UserBalanceVM>();

            foreach (var user in users)
            {
                decimal expensePaid = 0;
                decimal totalShare = 0;

                // User ne total kitna pay kiya
                expensePaid = expenses
                    .Where(x => x.PaidById == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                // Sirf un expenses ka share
                // jinke group ka member user hai
                foreach (var expense in expenses)
                {
                    var expenseGroup = groups.FirstOrDefault(g => g.GroupId == expense.GroupId);

                    if (expenseGroup == null)
                        continue;

                    var expenseGroupMembers = expenseGroup.Members.Split(',')
                        .Select(x => Convert.ToInt32(x.Trim()))
                        .ToList();

                    if (expenseGroupMembers.Contains(user.UserId))
                    {
                        totalShare += shares
                            .Where(x => x.ExpenseId == expense.ExpenseId && x.UserId == user.UserId)
                            .Sum(x => x.ShareAmount);
                    }
                }

                // User ne settlement me kitna diya
                decimal paidSettlement = settlements
                    .Where(x => x.FromUserId == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                // User ko settlement me kitna mila
                decimal receivedSettlement = settlements
                    .Where(x => x.ToUserId == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                decimal originalBalance = expensePaid - totalShare;
                decimal balance = originalBalance + paidSettlement - receivedSettlement;

                userBalances.Add(new UserBalanceVM
                {
                    UserId = user.UserId,
                    UserName = user.Name,
                    TotalPaid = Math.Round(expensePaid),
                    TotalShare = Math.Round(totalShare),
                    PaidSettlement = Math.Round(paidSettlement),
                    ReceivedSettlement = Math.Round(receivedSettlement),
                    Balance = Math.Round(balance)
                });
            }

            var hasGroupExpense = userBalances.Any(x => x.TotalPaid != 0 || x.TotalShare != 0);

            var pendingSettlements = new List<PendingSettlementVM>();

            if (hasGroupExpense)
            {
                pendingSettlements = CalculateSettlement(userBalances);
            }

            // Already Settled Records
            var history = db.Settlements.Include("FromUser").Include("ToUser").OrderByDescending(x => x.SettlementDate).ToList();

            // Remove Settled Amounts
            foreach (var item in pendingSettlements.ToList())
            {
                decimal settledAmount = history
                    .Where(x => x.FromUserId == item.FromUserId && x.ToUserId == item.ToUserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                item.Amount -= settledAmount;

                if (item.Amount <= 0)
                {
                    pendingSettlements.Remove(item);
                }

                item.CanSettle = item.FromUserId == currentUserId;
            }

            // Final Report Model
            var model = new ReportVM
            {
                TotalExpense = Math.Round(expenses.Sum(x => (decimal?)x.Amount) ?? 0),
                Users = userBalances.OrderBy(x => x.UserName).ToList(),
                PendingSettlement = pendingSettlements,
                SettlementHistory = history.Select(x => new SettlementHistoryVM
                {
                    FromUser = x.FromUser.Name,
                    ToUser = x.ToUser.Name,
                    Amount = x.Amount,
                    Date = x.SettlementDate
                }).ToList()
            };

            return View(model);
        }

        private List<PendingSettlementVM> CalculateSettlement(List<UserBalanceVM> users)
        {
            var result = new List<PendingSettlementVM>();

            int currentUserId = UserHelper.GetCurrentUserId();

            var creditors = users
                .Where(x => x.Balance > 0.01m)
                .Select(x => new SettlementUser
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Amount = x.Balance
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            var debtors = users
                .Where(x => x.Balance < -0.01m)
                .Select(x => new SettlementUser
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Amount = Math.Abs(x.Balance)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            int debtorIndex = 0;
            int creditorIndex = 0;

            while (debtorIndex < debtors.Count && creditorIndex < creditors.Count)
            {
                decimal amount = Math.Min(debtors[debtorIndex].Amount, creditors[creditorIndex].Amount);

                result.Add(new PendingSettlementVM
                {
                    FromUserId = debtors[debtorIndex].UserId,
                    ToUserId = creditors[creditorIndex].UserId,
                    FromUser = debtors[debtorIndex].UserName,
                    ToUser = creditors[creditorIndex].UserName,
                    Amount = Math.Round(amount),
                    CanSettle = debtors[debtorIndex].UserId == currentUserId
                });

                debtors[debtorIndex].Amount -= amount;
                creditors[creditorIndex].Amount -= amount;

                if (debtors[debtorIndex].Amount <= 0.01m)
                {
                    debtorIndex++;
                }

                if (creditors[creditorIndex].Amount <= 0.01m)
                {
                    creditorIndex++;
                }
            }

            return result;
        }

        public ActionResult Settle(int fromUserId = 0, int toUserId = 0, decimal amount = 0, string data = "")
        {
            if (!string.IsNullOrEmpty(data))
            {
                string decryptedData = UrlEncryptionHelper.Decrypt(data);
                var query = HttpUtility.ParseQueryString(decryptedData);
                fromUserId = Convert.ToInt32(query["fromUserId"]);
                toUserId = Convert.ToInt32(query["toUserId"]);
                amount = Convert.ToDecimal(query["amount"]);
            }

            int currentUserId = UserHelper.GetCurrentUserId();
            currentUserId = currentUserId == 0 ? fromUserId : currentUserId;

            // Sirf jis user ko payment karni hai
            // wahi settlement kar sakta hai.
            if (currentUserId != fromUserId)
            {
                TempData["ErrorMsg"] = "You are not allowed to settle this payment";
                if (!string.IsNullOrEmpty(data))
                {
                    return Json(new
                    {
                        success = false,
                        message = "You are not allowed to settle this payment"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return RedirectToAction("Index");
            }

            if (amount <= 0)
            {
                TempData["ErrorMsg"] = "Invalid settlement amount";
                if (!string.IsNullOrEmpty(data))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid settlement amount"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return RedirectToAction("Index");
            }

            // Current balances dobara calculate karo
            var users = db.Users.Where(x => x.IsActive == 1).ToList();

            var expenses = db.Expenses.ToList();

            var shares = db.ExpenseShares.ToList();

            var settlements = db.Settlements.Where(x => x.IsSettled).ToList();

            var balances = new List<UserBalanceVM>();

            foreach (var user in users)
            {
                decimal expensePaid = expenses
                    .Where(x => x.PaidById == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                decimal totalShare = shares
                    .Where(x => x.UserId == user.UserId)
                    .Sum(x => (decimal?)x.ShareAmount) ?? 0;

                decimal paidSettlement = settlements
                    .Where(x => x.FromUserId == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                decimal receivedSettlement = settlements
                    .Where(x => x.ToUserId == user.UserId)
                    .Sum(x => (decimal?)x.Amount) ?? 0;

                decimal originalBalance = expensePaid - totalShare;
                decimal balance = originalBalance + paidSettlement - receivedSettlement;

                balances.Add(new UserBalanceVM
                {
                    UserId = user.UserId,
                    UserName = user.Name,
                    TotalPaid = expensePaid,
                    TotalShare = totalShare,
                    PaidSettlement = paidSettlement,
                    ReceivedSettlement = receivedSettlement,
                    Balance = Math.Round(balance)
                });
            }

            var pending = CalculateSettlement(balances);

            var transaction = pending.FirstOrDefault(x => x.FromUserId == fromUserId && x.ToUserId == toUserId);

            if (transaction == null)
            {
                TempData["ErrorMsg"] = "This settlement is no longer pending";
                if (!string.IsNullOrEmpty(data))
                {
                    return Json(new
                    {
                        success = false,
                        message = "This settlement is no longer pending"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return RedirectToAction("Index");
            }

            if (amount > transaction.Amount)
            {
                TempData["ErrorMsg"] = "Settlement amount cannot be greater than pending amount";
                if (!string.IsNullOrEmpty(data))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Settlement amount cannot be greater than pending amount"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return RedirectToAction("Index");
            }

            var settlement = new Settlement
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Amount = Math.Round(amount),
                SettlementDate = DateTime.Now,
                IsSettled = true
            };

            db.Settlements.Add(settlement);
            db.SaveChanges();

            TempData["SuccessMsg"] = "Settlement completed successfully";

            if (!string.IsNullOrEmpty(data))
            {
                return Json(new
                {
                    success = true,
                    message = "Settlement completed successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return RedirectToAction("Index");
        }

        private class SettlementUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public decimal Amount { get; set; }
        }
    }
}