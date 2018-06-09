using BeeperCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class Parser
    {
        public List<Expression> ParseString(string complexExpression)
        {
            List<Expression> expressionsList = new List<Expression>();

            return expressionsList;
        }

        private List<Expression> ParseBrackets(string complexExpression)
        {
            List<Expression> expressionsList = new List<Expression>();
            int currentLBIndex = -1;
            int currentRBIndex = -1;
            string simpleExpression;
            while (complexExpression.IndexOf("(") != -1)
            {
                currentLBIndex = complexExpression.LastIndexOf("(");
                currentRBIndex = complexExpression.IndexOf(")");
                simpleExpression = complexExpression.Substring(currentLBIndex, currentRBIndex - currentLBIndex);
                expressionsList.AddRange(ParseSimpleExpressions(out simpleExpression));
                complexExpression = complexExpression.Substring(0, currentLBIndex) + simpleExpression + complexExpression.Substring(currentRBIndex);
            }

            return expressionsList;
        }

        private List<Expression> ParseSimpleExpressions(out string simpleExpressions)
        {
            List<Expression> expressionsList = new List<Expression>();
            simpleExpressions = "";

            return expressionsList;
        }
    }
}
