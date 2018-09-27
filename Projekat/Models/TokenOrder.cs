using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    [Table("TokenOrder")]
    public class TokenOrder
    {
        public Guid id { get; set; }
        public int userId { get; set; }
        public int numOfTokens { get; set; }
        public decimal price { get; set; }
        [Required]
        [StringLength(20)]
        public string status { get; set; }
        [Required]
        public string type { get; set; }
        public DateTime? dateSubmitted { get; set; }
        public virtual User user { get; set; }
    }
}