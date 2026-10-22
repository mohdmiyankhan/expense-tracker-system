using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class SetupViewModel
    {
        [Required(ErrorMessage = "Please enter room/group name")]
        [Display(Name = "Room / Group Name")]
        public string RoomName { get; set; }

        public List<SetupUserViewModel> Users { get; set; }

        public List<GroupOptionViewModel> Groups { get; set; }

        public SetupViewModel()
        {
            Users = new List<SetupUserViewModel>();
            Groups = new List<GroupOptionViewModel>();
        }
    }
}