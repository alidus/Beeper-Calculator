using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public enum CalcErrorCode {None, Success, DivideByZero, ParseFailure, InvalidModel}
    public enum DataErrorCode {UserIdentificationFailure, GettingHistoryFailure, DatabaseUpdateFailure}
    public class CalcViewModel
    {
        [Required]
        public string Expression { get; set; }
        [Required]
        public CalcErrorCode CalcErrorCode { get; set; }
        [Required]
        public List<DataErrorCode> DataErrorCodes { get; set; } = new List<DataErrorCode>();
        [Required]
        public List<Expression> Expressions { get; set; } = new List<Expression>();
    }
}