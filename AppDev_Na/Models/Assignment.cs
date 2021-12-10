using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDev_Na.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        [Required]
        public string TrainerId { get; set; }
        [ForeignKey("TrainerId")]
        public Trainer Trainer { get; set; }
    }
}