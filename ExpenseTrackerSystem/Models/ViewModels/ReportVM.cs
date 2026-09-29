using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class UserBalanceVM
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal TotalShare { get; set; }

        public decimal PaidSettlement { get; set; }

        public decimal ReceivedSettlement { get; set; }

        public decimal Balance { get; set; }

        public bool IsReceiver
        {
            get
            {
                return Balance > 0;
            }
        }
    }

    public class SettlementVM1
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public string FromUser { get; set; }
        public string ToUser { get; set; }

        public decimal Amount { get; set; }

        public bool CanSettle { get; set; }
    }

    public class ReportVM
    {
        public decimal TotalExpense { get; set; }

        public List<UserBalanceVM> Users { get; set; }

        public List<PendingSettlementVM> PendingSettlement { get; set; }

        public List<SettlementHistoryVM> SettlementHistory { get; set; }
    }

    public class PendingSettlementVM
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public string FromUser { get; set; }
        public string ToUser { get; set; }

        public decimal Amount { get; set; }

        public bool CanSettle { get; set; }
    }

    public class SettlementHistoryVM
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Remarks { get; set; }
    }
}