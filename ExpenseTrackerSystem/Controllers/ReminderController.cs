using ExpenseTrackerSystem.Data;
using ExpenseTrackerSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTrackerSystem.Controllers
{
    public class ReminderController : Controller
    {
        private readonly ExpenseDbContext db;

        public ReminderController()
        {
            db = new ExpenseDbContext();
        }

        [HttpPost]
        public ActionResult WhatsAppReminder(int fromUserId, int toUserId, decimal amount, string settlementData)
        {
            #region Generate Settlement Url
            string encryptedData = UrlEncryptionHelper.Encrypt(settlementData);

            string encryptedSettlementUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Expense/Settle?data=" + encryptedData;
            #endregion

            var users = db.Users.ToList();

            var fromUser = users.Where(x => x.UserId == fromUserId).FirstOrDefault();
            var toUser = users.Where(x => x.UserId == toUserId).FirstOrDefault();

            string mobile = "", message = "";

            mobile = "91" + fromUser.MobileNo;

            message = "Hi " + fromUser.Name + ",\n\n" +
                      "Just a friendly reminder that ₹" + "*" + amount + "*" + " is pending for settlement." + "\n\n" +
                      "Please complete the payment at your earliest convenience." + "\n\n" +
                      "Click the below link for settlement." + "\n" +
                      encryptedSettlementUrl + "\n\n" +
                      "Thank you." + "\n" +
                       toUser.Name;

            // + aur spaces remove
            mobile = mobile.Replace("+", "").Replace(" ", "");

            // Message URL Encode
            string encodedMessage = Uri.EscapeDataString(message);

            // WhatsApp URL
            string url = $"https://wa.me/{mobile}?text={encodedMessage}";

            return Json(url);
        }
    }
}