using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class SetupViewModel
    {
        [Required(ErrorMessage = "Room / Group Name is required")]
        [Display(Name = "Room / Group Name")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Total Member is required")]
        [Display(Name = "Total Member")]
        [Range(typeof(int), "2", "5", ErrorMessage = "Value must be greater than 0 and less than/equal 5")]
        public int TotalMember { get; set; }

        public List<SetupUserViewModel> Users { get; set; }

        public List<GroupOptionViewModel> Groups { get; set; }

        public SetupViewModel()
        {
            Users = new List<SetupUserViewModel>();
            Groups = new List<GroupOptionViewModel>();
        }
    }
}