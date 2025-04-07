using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string ProfileImage { get; set; }

        // Quan hệ một-nhiều với StudentCourse
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}


