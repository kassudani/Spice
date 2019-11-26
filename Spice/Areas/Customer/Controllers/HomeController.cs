using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsIdentitty = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentitty.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32("startSessionCartCount", cnt);
            }

            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var menuItemFronDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.MenuItemId == id);

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemFronDb,
                MenuItemId = menuItemFronDb.MenuItemId
            };

            return View(shoppingCart);

        }

        
        //details post
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart item)
        {
            item.ShoppingCartId = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                item.ApplicationUserId = claim.Value;
                ShoppingCart cartFromDb = await _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == item.ApplicationUserId && c.MenuItemId == item.MenuItemId).FirstOrDefaultAsync();
                if(cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(item);
                }
                else
                {
                    cartFromDb.Count += item.Count;
                }
                await _db.SaveChangesAsync();

                //this count is for session handeling for count of items in a cart
                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == item.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("startSessionCartCount", count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFronDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.MenuItemId == item.MenuItemId);

                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItemFronDb,
                    MenuItemId = menuItemFronDb.MenuItemId
                };
                return View(shoppingCart);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
