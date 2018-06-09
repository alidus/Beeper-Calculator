using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models.Repos
{
    public class UserExpressionRepository : DbRepository<UserExpression>
    {
        public UserExpressionRepository(DatabaseContext context) : base(context)
        {

        }

        public List<UserExpression> GetLinks(int UserID, int ExpressionID)
        {
            return DbSet.Where(l => l.UserID == UserID).Where(l => l.ExpressionID == ExpressionID).ToList();
        }

        public List<Expression> GetExpressionsForUser(User user)
        {
            return DbSet.Where(l => l.UserID == user.UserID).Select(l => l.Expression).ToList();
        }

        public IQueryable<UserExpression> GetLinksForUser(User user)
        {
            return DbSet.Where(l => l.UserID == user.UserID);
        }

        public List<Expression> GetExpressionsForUserAfterTime(User user, DateTime time)
        {
            return DbSet.Where(l => l.UserID == user.UserID).Where(l => DateTime.Compare(l.Time, time) > 0).OrderByDescending(l => l.Time).Select(l => l.Expression).ToList();
        }

        internal List<Expression> GetExpressionsForUserAfterTime(User user, object startOfDay)
        {
            throw new NotImplementedException();
        }
    }
}