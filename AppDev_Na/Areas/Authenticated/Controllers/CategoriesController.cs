using System.Linq;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppDev_Na.Areas.Authenticated
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Staff)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Categories.ToList());
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            Category category = _db.Categories.Find(id);
            if (category == null)
            {
                ViewData["Message"] = "Error: Categories of found";
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
            {
                return RedirectToAction(nameof(Index), "Categories");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            if (category.Id == 0)
            {
                var nameExist = _db.Categories.Where(c => c.Name == category.Name).ToList().Count() > 0;
                if (nameExist)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Categories.Add(category);
            }
            else
            {
                var nameExist = _db.Categories.Where(c => c.Name == category.Name && c.Id != category.Id).ToList().Count() > 0;
                if (nameExist)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Categories.Update(category);                
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Categories");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.Find(id);
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Categories");
        }
    }
}