using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class CourseRequests
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Users Student { get; set; }

        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Users Teacher { get; set; }

        public int CourseId {  get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
