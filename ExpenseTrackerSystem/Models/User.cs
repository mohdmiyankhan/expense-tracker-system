using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int IsActive { get; set; }

        public string MobileNo { get; set; }

        public string UPIId { get; set; }

        public virtual ICollection<Expense> ExpensesPaid { get; set; }

        public virtual ICollection<ExpenseShare> ExpenseShares { get; set; }
    }
}