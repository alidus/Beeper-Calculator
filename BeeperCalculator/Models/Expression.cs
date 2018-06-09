using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public class Expression
    {
        [Key]
        public int ExpressionID { get; set; }

        [Required]
        public string ExpressionString { get; set; }

        public virtual ICollection<UserExpression> UserExpressions { get; set; }

    }
}