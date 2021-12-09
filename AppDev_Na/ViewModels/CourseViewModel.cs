using System.Collections.Generic;
using AppDev_Na.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppDev_Na.ViewModels
{
    public class CourseViewModel 
    {
        // GET
        public IEnumerable<SelectListItem> Categories { get; set; }
        public Course Course { get; set; }
    }
}