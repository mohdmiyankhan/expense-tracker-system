using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class ExpenseVM
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "Value must be greater than 0")]
        public decimal Amount { get; set; }

        public int PaidById { get; set; }

        [Required(ErrorMessage = "Group is required")]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime ExpenseDate { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Groups { get; set; }
    }
}