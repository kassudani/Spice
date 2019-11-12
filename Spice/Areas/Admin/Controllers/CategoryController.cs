using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get action method- retreives data and pass to the view
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        //Get action method for create (It will only show the form with which we can create a new category)
        public IActionResult Create()
        {
            return View();
        }

        //Post method for create wher it will take the data from the user and post it in the db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //if the value is valid it will store in db
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Get for edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //post for edit
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Get for delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        //post for edit
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        //public async Task<IActionResult> ConfirmDelete(int id)

        //: we can write like this and with this we need to specify the action name in httppost
        //if we want same parameters we can change the name and specify the action name
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Category.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Get for Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
