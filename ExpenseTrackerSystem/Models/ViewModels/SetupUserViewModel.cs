using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class SetupUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Member Name")]
        public string Name { get; set; }

        public string Username { get; set; }

        public string MobileNo { get; set; }

        public string UPIId { get; set; }
    }
}