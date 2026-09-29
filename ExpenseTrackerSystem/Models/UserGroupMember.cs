using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Models
{
    public class UserGroupMember
    {
        public int UserGroupMemberId { get; set; }

        public int GroupId { get; set; }

        public int UserId { get; set; }

        public virtual UserGroup Group { get; set; }

        public virtual User User { get; set; }
    }
}