using System;
using System.ComponentModel.DataAnnotations;

namespace AppDev_Na.Models
{
    public class Trainer : ApplicationUser
    {
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Education { get; set; }
    }
}