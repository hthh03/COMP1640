namespace WebApplication2.Models
{
    public class Meetings
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public Students Students { get; set; }
        public string TeacherId { get; set; }
        public Teachers Teachers { get; set; }
    }
}
