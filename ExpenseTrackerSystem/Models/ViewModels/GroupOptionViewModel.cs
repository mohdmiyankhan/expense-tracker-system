using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models.ViewModels
{
    public class GroupOptionViewModel
    {
        public string GroupName { get; set; }

        public List<int> UserIds { get; set; }

        public bool IsSelected { get; set; }
    }
}