using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev_Na.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Trainee)]
    public class TraineeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TraineeController(ApplicationDbContext db)
        {
            _db = db;
        }

       
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var assignments = _db.Enrollments.Where(a => a.TraineeId == claims.Value);
            List<Course> courses = new List<Course>();
            foreach (var courserTrainers  in assignments)
            {
                var course = _db.Courses.Find(courserTrainers.CourseId);
                courses.Add(course);
            }

            return View(courses);
        }
        
        public IActionResult Details(int id)
        {
            var traineeInClass = _db.Enrollments.Where(s => s.CourseId == id).Include(i=>i.Trainee);
            return View(traineeInClass);
        }
    }
}