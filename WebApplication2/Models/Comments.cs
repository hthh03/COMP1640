using LoginDemo.Models;

namespace WebApplication2.Models
{
    public class Comments
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public Posts Posts { get; set; }
        public string UserId { get; set; }
        public Users Users { get; set; }
    }
}
