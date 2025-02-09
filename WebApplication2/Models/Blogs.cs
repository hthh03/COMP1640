namespace WebApplication2.Models
{
    public class Blogs
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public Users Users { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt {  get; set; }
        public ICollection<Posts> Posts { get; set; }
    }
}
