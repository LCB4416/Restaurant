using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Data
{
    public class OrderItem
    {
        [Required]
        public int OrderNo { get; set; }
        [Required]
        public int StockID { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
