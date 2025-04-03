using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "timestamp with time zone")] // Chỉ định kiểu dữ liệu trong PostgreSQL
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int BlogId { get; set; }

        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        // Đảm bảo UpdatedAt luôn là UTC
        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // Đảm bảo khi gán ngày giờ, chúng sẽ luôn là UTC
        public void SetCreatedAt(DateTime dateTime)
        {
            CreatedAt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public void SetUpdatedAt(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                UpdatedAt = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
            }
        }
    }
}
