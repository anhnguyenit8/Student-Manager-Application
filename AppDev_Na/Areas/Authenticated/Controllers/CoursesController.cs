using System.Collections.Generic;
using System.Linq;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using AppDev_Na.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppDev_Na.Areas.Authenticated
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Staff)]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CoursesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Courses.Include(c =>c.Category).ToList());
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            IEnumerable<Category> categoriesList = _db.Categories.ToList();
            CourseViewModel courseViewModel = new CourseViewModel()
            {
                Course = new Course(),
                Categories = categoriesList.Select(I => new SelectListItem()
                {
                    Text = I.Name,
                    Value = I.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(courseViewModel);
            }

            courseViewModel.Course = _db.Courses.Find(id);
            if (courseViewModel.Course == null)
            {
                return NotFound();
            }
            return View(courseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CourseViewModel courseViewModel)
        {
            if (courseViewModel == null)
            {
                return RedirectToAction(nameof(Index), "Courses");
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<Category> categoryList = _db.Categories.ToList();
                courseViewModel = new CourseViewModel()
                {
                    Course = new Course(),
                    Categories = categoryList.Select(I => new SelectListItem()
                    {
                        Text = I.Name,
                        Value = I.Id.ToString()
                    })
                };
                return RedirectToAction(nameof(Index));
            }

            if (courseViewModel.Course.Id == 0)
            {
                var nameExist = _db.Courses.Where(c => c.Name == courseViewModel.Course.Name).ToList().Count() > 0;
                if (nameExist)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Courses.Add(courseViewModel.Course);
            }
            else
            {
                var nameExist = _db.Courses.Where(c => c.Name == courseViewModel.Course.Name && c.Id != courseViewModel.Course.Id).ToList().Count() > 0;
                if (nameExist)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Courses.Update(courseViewModel.Course);                
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Courses");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _db.Courses.Find(id);
            _db.Courses.Remove(course);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}