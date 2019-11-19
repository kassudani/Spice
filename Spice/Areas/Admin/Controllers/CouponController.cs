using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)    
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //get create
        public IActionResult Create()
        {
            return View();
        }

        //post create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count() > 0)
                {
                    //file was uploaded
                    byte[] pic1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            pic1 = ms1.ToArray();
                        }
                    }
                    coupon.CouponImage = pic1;
                }
                _db.Coupon.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        //get edit
        public async Task<IActionResult> Edit (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.SingleOrDefaultAsync(c => c.CouponId == id);

            if(coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        //post edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {

            var couponFromDb = await _db.Coupon.Where(cdb => cdb.CouponId == coupon.CouponId).SingleOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] pic = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            pic = ms1.ToArray();
                        }
                    }
                    couponFromDb.CouponImage = pic;
                }
                couponFromDb.CouponName = coupon.CouponName;
                couponFromDb.CouponType = coupon.CouponType;
                couponFromDb.Discount = coupon.Discount;
                couponFromDb.MinimumAmount = coupon.MinimumAmount;
                couponFromDb.IsActive = coupon.IsActive;

                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.Where(c => c.CouponId == id).SingleOrDefaultAsync();

            if(coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var coupon = await _db.Coupon.Where(c => c.CouponId == id).SingleOrDefaultAsync();

            if (coupon == null)
            {
                return NotFound();
            }

            _db.Remove(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.Where(c => c.CouponId == id).SingleOrDefaultAsync();

            if(coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

    }
}