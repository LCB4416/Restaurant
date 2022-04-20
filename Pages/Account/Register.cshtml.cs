using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Restaurant.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegistrationModel Input { get; set; }

        private AppDbContext _db;
        public CheckoutCustomer Customer = new CheckoutCustomer();
        public Basket Basket = new Basket();

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,AppDbContext db)
            {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            }
      
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    NewBasket();
                    NewCustomer(Input.Email);
                    await _db.SaveChangesAsync();

                    return RedirectToPage("/Index");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
        public void NewBasket()
        {
            var currentBasket = _db.Baskets.FromSqlRaw("SELECT * From Baskets")
                .OrderByDescending(b => b.BasketID)
                .FirstOrDefault();
            if (currentBasket == null)
            {
                Basket.BasketID = 1;
            }
            else
            {
                Basket.BasketID = currentBasket.BasketID + 1;
            }
            _db.Baskets.Add(Basket);
        }

            public void NewCustomer(string Email)
            {
                Customer.Email = Email;
                Customer.BasketID = Basket.BasketID;
                _db.CheckoutCustomers.Add(Customer);
            }
        }
    }
