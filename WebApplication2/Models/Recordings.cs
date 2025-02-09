namespace WebApplication2.Models
{
    public class Recordings
    {
        public string Id { get; set; }
        public string MeetingId { get; set; }
        public Meetings Meetings { get; set; }
        public string TeacherId { get; set; }
        public Teachers Teachers { get; set; }
    }
}
