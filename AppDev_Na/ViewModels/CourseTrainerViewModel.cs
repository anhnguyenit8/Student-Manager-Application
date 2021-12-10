using Microsoft.AspNetCore.Mvc;
using AppDev_Na.Models;
using System.Collections.Generic;

namespace AppDev_Na.ViewModels
{
    public class CourseTrainerViewModel
    {
        public int CourseId { get; set; }
        public IEnumerable<Assignment> Assignment { get; set; }
        public IEnumerable<Trainer> Trainer { get; set; }
    }
}