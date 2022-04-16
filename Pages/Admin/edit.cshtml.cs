using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;

namespace Restaurant.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class editModel : PageModel
    {
        [BindProperty]
        public MenuFood item { get; set; }
        private readonly AppDbContext _db;
        public editModel(AppDbContext db) { _db = db; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            item = await _db.Menuitem.FindAsync(id);
            if(item== null)
            {
                return RedirectToPage("/Admin/Create");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _db.Attach(item).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"Item {item.ID} not found!", e);
            }
            return RedirectToPage("/Admin/Create");
        }
    }
}
