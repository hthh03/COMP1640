using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận là bắt buộc")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; }

        public int PostId { get; set; }
    }
}
