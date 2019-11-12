using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }
        private readonly ApplicationDbContext _db;
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s=> s.Category).ToListAsync();
            return View(subCategories);
        }

        //Get for create action method
        public async Task<IActionResult> Create()
        {
            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCategoryName)
                .Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(model);
        }

        //Post for create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSubCategoryExist = _db.SubCategory.Include(c => c.Category)
                    .Where(c => c.SubCategoryName == model.SubCategory.SubCategoryName &&
                    c.Category.CategoryId == model.SubCategory.CategoryId);
                if (isSubCategoryExist.Count() > 0)
                {
                    //error as the model we are trying to insert in db is already present
                    StatusMessage = "Error: SubCategory exists under" + isSubCategoryExist.First().Category.CategoryName + "category. Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelForViewModel = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(sc => sc.SubCategoryName).Select(sc => sc.SubCategoryName).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelForViewModel);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "SubCategoryId", "SubCategoryName"));
        }

        //Get for edit action method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(sc => sc.SubCategoryId == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCategoryName)
                .Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(model);
        }

        //post for edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSubCategoryExist = _db.SubCategory.Include(c => c.Category)
                    .Where(c => c.SubCategoryName == model.SubCategory.SubCategoryName &&
                    c.Category.CategoryId == model.SubCategory.CategoryId);
                if (isSubCategoryExist.Count() > 0)
                {
                    //error as the model we are trying to insert in db is already present
                    StatusMessage = "Error: SubCategory exists under" + isSubCategoryExist.First().Category.CategoryName + "category. Please use another name.";
                }
                else
                {
                    var subCategoryFromDb = await _db.SubCategory.FindAsync(id);
                    subCategoryFromDb.SubCategoryName = model.SubCategory.SubCategoryName;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelForViewModel = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(sc => sc.SubCategoryName).Select(sc => sc.SubCategoryName).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelForViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(sc => sc.SubCategoryId == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCategoryName)
                .Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(model);
        }

        //get for delete
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(sc => sc.SubCategoryId == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCategoryName)
                .Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(model);
        }

        //post for delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var subCategoryDelete = await _db.SubCategory.FindAsync(id);
            if(subCategoryDelete == null)
            {
                return NotFound();
            }
            _db.Remove(subCategoryDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}