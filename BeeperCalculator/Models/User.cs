using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string UserIP { get; set; }

        public virtual ICollection<UserExpression> UserExpressions { get; set; }

    }
}