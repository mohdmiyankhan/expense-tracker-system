using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class Settlement
    {
        public int SettlementId { get; set; }

        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public decimal Amount { get; set; }

        public DateTime SettlementDate { get; set; }

        public bool IsSettled { get; set; }

        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; }

        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
    }
}