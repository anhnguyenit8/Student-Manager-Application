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
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static int CourseId;

        public EnrollmentsController(ApplicationDbContext db)
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
                return RedirectToAction(nameof(CourseTrainee));
            }
            return RedirectToAction(nameof(CourseTrainee));
        }

        public IActionResult CourseTrainee()
        {
            CourseTraineeViewModel courseTraineeViewModel = new CourseTraineeViewModel();
            courseTraineeViewModel.CourseId = CourseId;
            courseTraineeViewModel.Enrollments = _db.Enrollments.ToList();
            courseTraineeViewModel.Trainee = _db.Trainee.ToList();
            return View(courseTraineeViewModel);
        }

        public IActionResult Enroll(string id)
        {
            Enrollment traineeEnroll = new Enrollment()
            {
                TraineeId = id,
                CourseId = CourseId
            };
            var isExist = _db.Enrollments.Where(i => i.CourseId == CourseId && i.TraineeId == id);
            if (!isExist.Any())
            {
                _db.Enrollments.Add(traineeEnroll);
            }

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string Id)
        {
            var traineeEnroll = _db.Enrollments.Where(i => i.CourseId == CourseId && i.TraineeId == Id);
            {
                if (Id == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                _db.Enrollments.Remove(traineeEnroll.FirstOrDefault());
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }


    }
}