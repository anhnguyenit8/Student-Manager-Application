using Microsoft.AspNetCore.Mvc;
using AppDev_Na.Models;
using System.Collections.Generic;

namespace AppDev_Na.ViewModels
{
    public class CourseTraineeViewModel
    {
        public int CourseId { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public IEnumerable<Trainee> Trainee { get; set; }
    }
}