using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Data
{
    public class OrderHistories
    {
        [Key, Required]
        public int OrderNo { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
