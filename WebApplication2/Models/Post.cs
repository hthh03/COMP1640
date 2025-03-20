using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Mặc định là UTC NOW

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;  // Mặc định là UTC NOW

        [Required]
        public int BlogId { get; set; }

        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
