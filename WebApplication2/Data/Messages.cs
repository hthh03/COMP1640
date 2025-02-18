using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class Messages
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public Students Students { get; set; }
        public string TeacherId { get; set; }
        public Teachers Teachers { get; set; }
    }
}
