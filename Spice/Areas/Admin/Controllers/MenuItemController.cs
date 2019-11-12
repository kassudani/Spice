using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel menuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            menuItemVM = new MenuItemViewModel
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var categoires = _db.Category.ToList();
            var menuItems = await _db.MenuItem.Include(m => m.SubCategory).Include(m=>m.Category).ToListAsync();
            return View(menuItems);
        }

        //Get- Create
        public IActionResult Create()
        {
            return View(menuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost ()
        {
            var text = Request.Form["SubCategoryId"].ToString();
            menuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(menuItemVM);
            }

            _db.MenuItem.Add(menuItemVM.MenuItem);
            await _db.SaveChangesAsync();


            // image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFomDb = await _db.MenuItem.FindAsync(menuItemVM.MenuItem.MenuItemId);

            if(files.Count() > 0)
            {
                //file has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using(var fileStream = new FileStream(Path.Combine(uploads, menuItemVM.MenuItem.MenuItemId + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFomDb.Image = @"\images\" + menuItemVM.MenuItem.MenuItemId + extension;
            }
            else
            {
                //no file is uploaded, we will use a default one
                var uploads = Path.Combine(webRootPath, @"images\" + StaticDetail.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + menuItemVM.MenuItem.MenuItemId + ".png");
                menuItemFomDb.Image = @"\images\" + menuItemVM.MenuItem.MenuItemId + ".png";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        //get edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            menuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.MenuItemId == id);
            menuItemVM.SubCategory = await _db.SubCategory.Where(m => m.CategoryId == menuItemVM.MenuItem.CategoryId)
                .ToListAsync();

            if(menuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            ViewBag.defaultImage = @"\\images\\default_food.png";
            return View(menuItemVM);
        }

        //post edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            var text = Request.Form["SubCategoryId"].ToString();
            menuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                menuItemVM.SubCategory = await _db.SubCategory.Where(m => m.CategoryId == menuItemVM.MenuItem.CategoryId).ToListAsync(); 
                return View(menuItemVM);
            }

            
            // image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFomDb = await _db.MenuItem.FindAsync(menuItemVM.MenuItem.MenuItemId);

            if (files.Count() > 0)
            {
                //New img has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var newExtension = Path.GetExtension(files[0].FileName);

                //deleting the old img
                var imgPath = Path.Combine(webRootPath, menuItemFomDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }

                //uploading new one
                using (var fileStream = new FileStream(Path.Combine(uploads, menuItemVM.MenuItem.MenuItemId + newExtension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFomDb.Image = @"\images\" + menuItemVM.MenuItem.MenuItemId + newExtension;
            }

            menuItemFomDb.MenuItemName = menuItemVM.MenuItem.MenuItemName;
            menuItemFomDb.Description = menuItemVM.MenuItem.Description;
            menuItemFomDb.Price = menuItemVM.MenuItem.Price;
            menuItemFomDb.Spiciness = menuItemVM.MenuItem.Spiciness;
            menuItemFomDb.CategoryId = menuItemVM.MenuItem.CategoryId;
            menuItemFomDb.SubCategoryId = menuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            menuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.MenuItemId == id);
            menuItemVM.SubCategory = await _db.SubCategory.Where(m => m.CategoryId == menuItemVM.MenuItem.CategoryId)
                .ToListAsync();

            if (menuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(menuItemVM);
        }

        //post delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {

            if (!ModelState.IsValid)
            {
                menuItemVM.SubCategory = await _db.SubCategory.Where(m => m.CategoryId == menuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(menuItemVM);
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFomDb = await _db.MenuItem.FindAsync(menuItemVM.MenuItem.MenuItemId);

            if (files.Count() > 0)
            {
                //deleting the old img
                var imgPath = Path.Combine(webRootPath, menuItemFomDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

            _db.Remove(menuItemFomDb);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            menuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.MenuItemId == id);
            menuItemVM.SubCategory = await _db.SubCategory.Where(m => m.CategoryId == menuItemVM.MenuItem.CategoryId)
                .ToListAsync();

            if (menuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(menuItemVM);
        }
    }
}