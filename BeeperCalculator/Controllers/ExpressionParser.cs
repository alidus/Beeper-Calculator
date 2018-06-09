using BeeperCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeperCalculator.Controllers
{
    static public class ExpressionParser
    {
        static public float ParseString(string complexExpression)
        {
            string resultString = null;
            try
            {
                complexExpression = RemoveSpaces(complexExpression);
                resultString = ParseBrackets(complexExpression);
                return GetFloat(resultString);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        static public string RemoveSpaces(string str)
        {
            return str.Replace(" ", string.Empty);
        }

        static private string ParseBrackets(string complexExpression)
        {
            int currentLBIndex = -1;
            int currentRBIndex = -1;
            string simpleExpression = complexExpression;
            while (complexExpression.IndexOf("(") != -1)
            {
                currentLBIndex = complexExpression.LastIndexOf("(") + 1;
                currentRBIndex = complexExpression.IndexOf(")");
                simpleExpression = complexExpression.Substring(currentLBIndex, currentRBIndex - currentLBIndex);
                if (simpleExpression[0] == '-') // If first element in brackets is negative
                {
                    if (GetNumOfOperators(simpleExpression) == 1)
                    {
                        // Single operator (-) in brackets
                        simpleExpression = "{" + simpleExpression.Substring(1) + "}";
                    } else
                    {
                        // Multiple operators in brackets
                        simpleExpression = "{" + simpleExpression.Substring(1, GetFirstOperatorIndex(simpleExpression.Substring(1))) + "}" +
                        simpleExpression.Substring(GetFirstOperatorIndex(simpleExpression.Substring(1)) + 1);
                    }
                    
                }
                simpleExpression = ParseSimpleExpressions(simpleExpression);
                complexExpression = complexExpression.Substring(0, currentLBIndex - 1) + simpleExpression + complexExpression.Substring(currentRBIndex + 1);
            }
        
            return ParseSimpleExpressions(complexExpression);
        }

        static private string ParseSimpleExpressions(string simpleExpression)
        {
            int tempMulIndex = simpleExpression.IndexOf("*");
            int tempDivIndex = simpleExpression.IndexOf("/");

            // Folding for mul and div operators
            while (tempDivIndex != -1 || tempMulIndex != -1)
            {
                if (tempDivIndex != -1 && tempMulIndex != -1)
                {
                    simpleExpression = FoldOperator(simpleExpression, Math.Min(tempDivIndex, tempMulIndex) == tempDivIndex ? "/" : "*");
                } else if (tempDivIndex != -1)
                {
                    simpleExpression = FoldOperator(simpleExpression, "/");
                } else
                {
                    simpleExpression = FoldOperator(simpleExpression, "*");
                }
                tempDivIndex = simpleExpression.IndexOf("/");
                tempMulIndex = simpleExpression.IndexOf("*");
            }

            int tempAddIndex = simpleExpression.IndexOf("+");
            int tempSubIndex = simpleExpression.IndexOf("-"); 

            // Folding for add and sub operators
            while (tempAddIndex != -1 || tempSubIndex != -1)
            {
                if (tempAddIndex != -1 && tempSubIndex != -1)
                {
                    simpleExpression = FoldOperator(simpleExpression, Math.Min(tempAddIndex, tempSubIndex) == tempAddIndex ? "+" : "-");
                }
                else if (tempAddIndex != -1)
                {
                    simpleExpression = FoldOperator(simpleExpression, "+");
                }
                else
                {
                    simpleExpression = FoldOperator(simpleExpression, "-");
                }
                tempAddIndex = simpleExpression.IndexOf("+");
                tempSubIndex = simpleExpression.IndexOf("-");
            }

            return simpleExpression;
        }

        /// <summary>
        /// Fold first found single-operator block of expression of a given operator 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        static private string FoldOperator(string expression, string oper)
        {
            int tempIndexOfOperator;
            int tempIndexOfLeftOpearator;
            int tempIndexOfRightOpearator;
            float tempLeftValue;
            float tempRightValue;
            float tempResult;
            string resultString = expression;

            tempIndexOfOperator = resultString.IndexOf(oper);
            tempIndexOfLeftOpearator = GetLastOperatorIndex(resultString.Substring(0, tempIndexOfOperator));
            tempIndexOfRightOpearator = GetFirstOperatorIndex(resultString.Substring(tempIndexOfOperator + 1)) + tempIndexOfOperator + 1;

            // There is no operator on the left
            if (tempIndexOfLeftOpearator == -1) 
            {
                tempLeftValue = GetFloat(resultString.Substring(0, tempIndexOfOperator));
            } else
            {
                tempLeftValue = GetFloat(resultString.Substring(tempIndexOfLeftOpearator + 1, tempIndexOfOperator - tempIndexOfLeftOpearator - 1));
            }

            // If there is no operator on the right
            if (tempIndexOfRightOpearator == tempIndexOfOperator)   
            {

                tempRightValue = GetFloat(resultString.Substring(tempIndexOfOperator + 1));
                tempIndexOfRightOpearator = -1;
            } else
            {
                tempRightValue = GetFloat(resultString.Substring(tempIndexOfOperator + 1, tempIndexOfRightOpearator - tempIndexOfOperator - 1));
            }

            if (tempRightValue == 0) { throw new DivideByZeroException(); }

            switch (oper)
            {
                case "*":
                    tempResult = tempLeftValue * tempRightValue;
                    break;
                case "/":
                    tempResult = tempLeftValue / tempRightValue;
                    break;
                case "+":
                    tempResult = tempLeftValue + tempRightValue;
                    break;
                case "-":
                    tempResult = tempLeftValue - tempRightValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            resultString = (tempIndexOfLeftOpearator == -1 ? "" : expression.Substring(0, tempIndexOfLeftOpearator + 1));
            resultString += (tempResult >= 0 ? tempResult.ToString() : "{" + (-tempResult).ToString() + "}");
            resultString += (tempIndexOfRightOpearator == -1 ? "" : expression.Substring(tempIndexOfRightOpearator));
            expression = resultString;

            return resultString;
        }

        static private float GetFloat(string value)
        {
            try
            {
                return value[0] == '{' ? -float.Parse(value.Substring(1, value.Length - 2)) : float.Parse(value);
            }
            catch (Exception)
            {
                throw new ArithmeticException();
            }
            
        }

        static private int GetLastOperatorIndex(string expression)
        {
            return Math.Max(Math.Max(expression.LastIndexOf("/"), expression.LastIndexOf("*")), Math.Max(expression.LastIndexOf("+"), expression.LastIndexOf("-")));
        }

        static private int GetNumOfOperators(string expression)
        {
            return expression.Count(c => c == '-') + expression.Count(c => c == '+') + 
                expression.Count(c => c == '/') + expression.Count(c => c == '*');
        }

        static private int GetFirstOperatorIndex(string expression)
        {
            List<int> indexes = new List<int> { expression.IndexOf("*"), expression.IndexOf("/"),
                                                expression.IndexOf("+"), expression.IndexOf("-")};
            var existingOperators = indexes.Where((int index) => index != -1).ToList();
            if (existingOperators.Count  > 0)
            {
                return existingOperators.Min();
            } else
            {
                return -1;
            }
            
        }
    }
}
