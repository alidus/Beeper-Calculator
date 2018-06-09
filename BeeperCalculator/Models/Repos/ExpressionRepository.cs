using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models.Repos
{
    public class ExpressionRepository : DbRepository<Expression>
    {
        public ExpressionRepository(DatabaseContext context) : base(context)
        {

        }

        public List<Expression> GetWithExpressionString(string expressionString)
        {
            return DbSet.Where(e => e.ExpressionString == expressionString).OrderBy(e => e.ExpressionID).ToList();
        }

    }
}