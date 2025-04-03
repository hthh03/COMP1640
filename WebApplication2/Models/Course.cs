using WebApplication2.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string TeacherId { get; set; } // Giáo viên tạo khóa học
    public Users Teacher { get; set; }
    //public List<StudentCourse> StudentCourses { get; set; } // Học sinh tham gia

    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
