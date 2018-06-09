using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Expression> Expressions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserExpression> UserExpressions { get; set; }

        public DatabaseContext() : base("BeeperCalcContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}