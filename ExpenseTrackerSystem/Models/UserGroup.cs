using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class UserGroup
    {
        [Key]
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Members { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }
    }
}