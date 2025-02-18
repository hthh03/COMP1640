using WebApplication2.Models;

namespace WebApplication2.Data
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
