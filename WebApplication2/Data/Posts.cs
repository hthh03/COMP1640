using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class Posts
    {
        public string Id { get; set; }
        public string BlogId { get; set; }
        public Blogs Blogs { get; set; }
        public string userId { get; set; }
        public Users Users { get; set; }
        public ICollection<Comments> Comments { get; set; }
    }
}
