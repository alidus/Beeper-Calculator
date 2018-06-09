using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models.Repos
{
    public class UserRepository : DbRepository<User>
    {
        public UserRepository(DatabaseContext context) : base(context)
        {

        }

        public List<User> GetWithIP(string ip)
        {
            return DbSet.Where(u => u.UserIP.Contains(ip)).OrderBy(u => u.UserID).ToList();
        }
    }
}