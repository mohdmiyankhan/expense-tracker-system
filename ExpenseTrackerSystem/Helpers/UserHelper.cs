using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Helpers
{
    public static class UserHelper
    {
        public static int GetCurrentUserId()
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                return 0;
            }

            return (int)HttpContext.Current.Session["UserId"];
        }
    }
}