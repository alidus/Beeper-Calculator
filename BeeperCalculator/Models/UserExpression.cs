using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public class UserExpression
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Expression")]
        public int ExpressionID { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public virtual User User { get; set; }
        public virtual Expression Expression { get; set; }
    }
}