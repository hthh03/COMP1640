using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class PostViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Tiêu đề bài viết là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung bài viết là bắt buộc")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int BlogId { get; set; }

        public string BlogTitle { get; set; }

        public string AuthorName { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}
