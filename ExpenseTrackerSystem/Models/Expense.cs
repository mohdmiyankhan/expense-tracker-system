using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public int PaidById { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("PaidById")]
        public virtual User PaidBy { get; set; }

        public virtual UserGroup Group { get; set; }

        public virtual ICollection<ExpenseShare> Shares { get; set; }
    }
}