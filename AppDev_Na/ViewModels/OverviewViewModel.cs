using System.Collections.Generic;
using AppDev_Na.Models;


namespace AppDev_Na.ViewModels
{
    public class OverviewViewModel 
    {
        // GET
        public IEnumerable<Assignment> TrainerList { get; set; }
        public IEnumerable<Enrollment> TraineeList { get; set; }
        
    }
}