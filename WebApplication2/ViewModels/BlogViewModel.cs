using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class BlogViewModel
    {
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Tiêu đề blog là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả blog là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; }

        public string UserName { get; set; }

        public string UserRole { get; set; }
    }
}
