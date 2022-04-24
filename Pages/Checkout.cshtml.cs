using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Stripe;

namespace Restaurant.Pages
{
    public class CheckoutModel : PageModel
    {
        public OrderHistories Order = new OrderHistories();
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<CheckoutItems> MenuItems { get; private set; }
        public decimal Total = 0;
        public long AmountPayable = 0;

        public CheckoutModel(AppDbContext db, UserManager<ApplicationUser> UserManager)
        {
            _db = db;
            _UserManager = UserManager;

        }
        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            MenuItems = _db.CheckoutItems.FromSqlRaw(
                "SELECT Menuitem.ID, Menuitem.Price, " +
                "Menuitem.MenuName, " +
                "BasketItems.BasketID, BasketItems.Quantity " +
                "FROM Menuitem INNER JOIN BasketItems " +
                "ON menuitem.ID = BasketItems.StockID " +
                "WHERE BasketID = {0}", customer.BasketID
                ).ToList();

            Total = 0;
            foreach (var item in MenuItems)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)(Total * 100);
        }

        public async Task<IActionResult> OnPostPurchaseAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            var item = _db.BasketItems.FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", itemID, customer.BasketID)
                        .ToList()
                        .FirstOrDefault();


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
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDecreaseAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User);
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
                item.Quantity = item.Quantity - 1;

                if (item.Quantity == 0)
                {
                    _db.BasketItems.Remove(item);
                }
                else
                {
                    _db.Attach(item).State = EntityState.Modified;
                }

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

        public async Task Process()
        {
            var currentOrder = _db.OrderHistories
  .FromSqlRaw("SELECT * From OrderHistories")
                .OrderByDescending(b => b.OrderNo)
                .FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

            var basketItems =
                _db.BasketItems
                .FromSqlRaw("SELECT * From BasketItems " +
                "WHERE BasketID = {0}", customer.BasketID)
                .ToList();

            foreach (var item in basketItems)
            {
               Data.OrderItem oi = new Data.OrderItem
                {
                    OrderNo = Order.OrderNo,
                    StockID = item.StockID,
                    Quantity = item.Quantity
                };
                _db.OrderItems.Add(oi);
                _db.BasketItems.Remove(item);
            }

            await _db.SaveChangesAsync();
        }

        public IActionResult OnPostCharge(
string stripeEmail,
string stripeToken,
long amount)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = amount,
                Description = "CO5227 Stationaries Charge",
                Currency = "sgd",
                Customer = customer.Id
            });
            Process().Wait();
            return RedirectToPage("/Index");
        }


    }

}



