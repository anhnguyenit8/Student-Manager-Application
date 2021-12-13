using System;
using System.ComponentModel.DataAnnotations;

namespace AppDev_Na.Models
{
    public class Trainee : ApplicationUser
    {
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Education { get; set; }
    }
}