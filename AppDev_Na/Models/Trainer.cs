using System;
using System.ComponentModel.DataAnnotations;

namespace AppDev_Na.Models
{
    public class Trainer : ApplicationUser
    {
        [Required]
        public string Specialty { get; set; }
    }
}