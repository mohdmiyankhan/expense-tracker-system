using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class ExpenseShare
    {
        [Key]
        public int ShareId { get; set; }

        public int ExpenseId { get; set; }

        public int UserId { get; set; }

        public decimal ShareAmount { get; set; }

        public virtual Expense Expense { get; set; }

        public virtual User User { get; set; }
    }
}