using System;
using System.Linq;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using AppDev_Na.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev_Na.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Staff)]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static int CourseId;

        public AssignmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Courses.Include(c => c.Category).ToList());
        }

        public IActionResult ChooseCourse(int? id)
        {
            if (id != null)
            {
                CourseId = Convert.ToInt32(id);
                return RedirectToAction(nameof(CourseTrainer));
            }
            return RedirectToAction(nameof(CourseTrainer));
        }

        public IActionResult CourseTrainer()
        {
            CourseTrainerViewModel courseTrainerViewModel = new CourseTrainerViewModel();
            courseTrainerViewModel.CourseId = CourseId;
            courseTrainerViewModel.Assignment = _db.Assignments.ToList();
            courseTrainerViewModel.Trainer = _db.Trainer.ToList();
            return View(courseTrainerViewModel);
        }

        public IActionResult Assign(string id)
        {
            Assignment trainerAssign = new Assignment()
            {
                TrainerId = id,
                CourseId = CourseId
            };
            var isExist = _db.Assignments.Where(i => i.CourseId == CourseId && i.TrainerId == id);
            if (!isExist.Any())
            {
                _db.Assignments.Add(trainerAssign);
            }

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string Id)
        {
            var trainerAssign = _db.Assignments.Where(i => i.CourseId == CourseId && i.TrainerId == Id);
            {
                if (Id == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Assignments.Remove(trainerAssign.FirstOrDefault());
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }


    }
}