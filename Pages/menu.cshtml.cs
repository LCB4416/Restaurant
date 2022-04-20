using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Restaurant.Pages
{
   
    public class menuModel : PageModel
    {
        private readonly AppDbContext _db;
        public IList <MenuFood> Restaurant2022 { get; set; }
        [BindProperty]
        public string Search { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        public menuModel (AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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
        public async Task<IActionResult> OnPostBuyAsync(int itemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            var item = _db.BasketItems.FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", itemID, customer.BasketID)
                        .ToList()
                        .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    StockID = itemID,
                    Quantity = 1
                };
                _db.BasketItems.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found!", e);
                }
            }
            return RedirectToPage();
        }
    }

}

    
