using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Data
{
    public class Basket
    {
        [Key]
        public int BasketID { get; set; }

    }
}
