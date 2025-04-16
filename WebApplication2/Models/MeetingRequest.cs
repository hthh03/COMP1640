namespace WebApplication2.Models
{
    public class MeetingRequest
    {
        public int Id { get; set; }
        public string StudentEmail { get; set; }
        public string TeacherEmail { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }        // Thêm thuộc tính này
        public DateTime PreferredTime { get; set; }
        public DateTime MeetingTime { get; set; }  // Thêm thuộc tính này
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}