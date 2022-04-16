using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;

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
            foreach (var file in Request.Form.Files)
            {
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                Food.ImageData = ms.ToArray();

                ms.Close();
                ms.Dispose();
            }
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
