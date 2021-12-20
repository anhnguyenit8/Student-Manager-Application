using System.Linq;
using AppDev_Na.Data;
using AppDev_Na.Utility.Enum;
using AppDev_Na.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev_Na.Areas.Authenticated
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Staff)]
    public class OverViewController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public OverViewController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var allcourse = _db.Courses.Include(c => c.Category).ToList();
            return View(allcourse);
        }

        public IActionResult Overview(int? id)
        {
            var trainerList = _db.Assignments.Include(l => l.Trainer).Where(u => u.CourseId == id);
            var traineeList = _db.Enrollments.Include(l => l.Trainee).Where(u => u.CourseId == id);
            OverviewViewModel overviewViewModel = new OverviewViewModel()
            {
                TrainerList = trainerList,
                TraineeList = traineeList
            };
            return View(overviewViewModel);
        }
    }
}