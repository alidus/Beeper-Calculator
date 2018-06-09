using BeeperCalculator.Models;
using BeeperCalculator.Models.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BeeperCalculator.Controllers
{
    
    public class HomeController : Controller
    {
        DatabaseContext context = new DatabaseContext();
        ExpressionRepository expRepo;
        UserRepository userRepo;
        UserExpressionRepository userExpRepo;

        string expressionResult;
        string currentUserIP;
        User currentUser;
        Expression currentExpression;

        public HomeController()
        {
            expRepo = new ExpressionRepository(context);
            userRepo = new UserRepository(context);
            userExpRepo = new UserExpressionRepository(context);
        }

        public ActionResult Index()
        {
            var model = new CalcViewModel();
            model.CalcErrorCode = CalcErrorCode.None;
            model.Expression = "";

            try
            {
                SetUpCurrentUser();

                try
                {
                    // Try to load user expressions history 
                    model.Expressions = GetTodayExpressionsHistoryForUser(currentUser);
                }
                catch (Exception)
                {
                    // Unable to load history, preparing empty model

                    model.DataErrorCodes.Add(DataErrorCode.GettingHistoryFailure);
                    model.Expressions = new List<Expression>();
                    model.Expression = "";
                }

            }
            catch (Exception)
            {
                // Unable to identify user
                model.DataErrorCodes.Add(DataErrorCode.UserIdentificationFailure);
            }

            return View(model: model);
            
        }


        [HttpPost]
        public ActionResult Index(CalcViewModel model)
        {
            try
            {
                SetUpCurrentUser();
            }
            catch (Exception)
            {
                // Unable to identify user
                model.DataErrorCodes.Add(DataErrorCode.UserIdentificationFailure);
            }

            if (ModelState.IsValid)
            {
                expressionResult = model.Expression;
                // Parse expression, get history, update db
                try
                {
                    expressionResult = ExpressionParser.ParseString(model.Expression).ToString();
                    model.CalcErrorCode = CalcErrorCode.Success;

                    // If user identified
                    if (!model.DataErrorCodes.Contains(DataErrorCode.UserIdentificationFailure))
                    {
                        // Make changes in entites for current user/expression
                        try
                        {
                            SetUpCurrentExpression(model.Expression);
                            UpdateLink();
                            UpdateDatabase();
                        }
                        catch (Exception)
                        {
                            // Failure while performing db update
                            model.DataErrorCodes.Add(DataErrorCode.DatabaseUpdateFailure);
                        }
                    }
                }
                catch (DivideByZeroException)
                {
                    model.CalcErrorCode = CalcErrorCode.DivideByZero;
                }

                catch (Exception)
                {
                    // Failure while calculating
                    model.CalcErrorCode = CalcErrorCode.ParseFailure;
                }

            model.Expression = expressionResult;
            } else
            {
                // Model not valid
                model.CalcErrorCode = CalcErrorCode.InvalidModel;
            }

            try
            {
                // Try to get expressions history for user
                model.Expressions = GetTodayExpressionsHistoryForUser(currentUser);
            }
            catch (Exception)
            {
                // Unable to get history, setting up empty history
                model.DataErrorCodes.Add(DataErrorCode.GettingHistoryFailure);
            }

            return View(model : model);
        }

        private void SetUpCurrentUser()
        {
            currentUserIP = Request.UserHostAddress;
            List<User> usersInDbWithCurrentIP = userRepo.GetWithIP(currentUserIP);
            if (usersInDbWithCurrentIP.Count == 0)
            {
                // No users with current IP in db
                currentUser = new User { UserIP = currentUserIP };
                userRepo.Add(currentUser);
            }
            else if (usersInDbWithCurrentIP.Count == 1)
            {
                // Single user with current IP in db
                currentUser = usersInDbWithCurrentIP[0];
            }
            else
            {
                // Multiple users with the same current IP in db
                // Picking the one with larger ID
                currentUser = usersInDbWithCurrentIP[usersInDbWithCurrentIP.Count - 1];
            }
        }

        private void SetUpCurrentExpression(string expression)
        {
            List<Expression> expressionsInDbWithCurrentExpString = expRepo.GetWithExpressionString(expression);
            if (expressionsInDbWithCurrentExpString.Count == 0)
            {
                // No expressions with current exp string in db
                currentExpression = new Expression { ExpressionString = expression };
                expRepo.Add(currentExpression);
            }
            else if (expressionsInDbWithCurrentExpString.Count == 1)
            {
                // Single expression with current exp string in db
                currentExpression = expressionsInDbWithCurrentExpString[0];
            }
            else
            {
                // Multiple expressions with the same current exp string in db
                // Picking the one with larger ID
                currentExpression = expressionsInDbWithCurrentExpString[expressionsInDbWithCurrentExpString.Count - 1];
            }
        }

        private void UpdateLink()
        {
            List<UserExpression> links = userExpRepo.GetLinks(currentUser.UserID, currentExpression.ExpressionID);
            // If the same expression was calculated with the same user
            if (links.Count == 1)
            {
                // Remove old link
                userExpRepo.Remove(links[0]);
                // Add new with updated time
            }

            userExpRepo.Add(new UserExpression
            {
                User = currentUser,
                Expression = currentExpression,
                Time = DateTime.Now
            });
        }

        private void UpdateDatabase()
        {
            expRepo.SaveChanges();
            userRepo.SaveChanges();
            userExpRepo.SaveChanges();
        }

        private List<Expression> GetTodayExpressionsHistoryForUser(User user)
        {
            return userExpRepo.GetExpressionsForUserAfterTime(user, StartOfDay(DateTime.Now));
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Check if expression is exist and update link for current user,
        /// write data to db, 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
    }
}