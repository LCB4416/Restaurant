using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data;
using Microsoft.EntityFrameworkCore;

namespace Restaurant.Pages
{
   
    public class menuModel : PageModel
    {
        private readonly AppDbContext _db;
        public IList <MenuFood> Restaurant2022 { get; set; }
        [BindProperty]
        public string Search { get; set; }
        public menuModel (AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Restaurant2022 = _db.Menuitem.FromSqlRaw(
                "SELECT * FROM Menuitem WHERE Active = 1"
                ).ToList();
        }

        public IActionResult OnPostSearch()
        {
            Restaurant2022 = _db.Menuitem.FromSqlRaw(
              "SELECT * FROM Menuitem WHERE Active = 1 AND MenuName LIKE '%" +  Search +"%'"
              ).ToList();
            return Page();
        }
    }
}
