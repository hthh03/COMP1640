using WebApplication2.Models;

public class Submission
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public Users Student { get; set; }
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; }
    public string FilePath { get; set; } // Lưu đường dẫn file nộp bài
    public DateTime SubmittedAt { get; set; }
}
