using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data;
using Microsoft.AspNetCore.Authorization;

namespace Restaurant.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private AppDbContext _db;
        [BindProperty]
        public MenuFood Food { get; set; }

        public CreateModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }
            Food.Active = true;
            _db.Menuitem.Add(Food);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Index");
            //return Page();
        }

        public void OnGet()
        {
        }
    }
}
