using ExpenseTrackerSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExpenseTrackerSystem.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext() : base("ExpenseDbContext")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<ExpenseShare> ExpenseShares { get; set; }

        public DbSet<UserGroupMember> UserGroupMembers { get; set; }

        public DbSet<Settlement> Settlements { get; set; }
    }
}