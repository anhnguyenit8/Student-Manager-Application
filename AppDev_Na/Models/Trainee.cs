using System.ComponentModel.DataAnnotations;

namespace AppDev_Na.Models
{
    public class Trainee : ApplicationUser
    {
        [Required]
        public string Specialty { get; set; }
    }
}