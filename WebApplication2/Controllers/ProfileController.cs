using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(Users model, IFormFile ProfileImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Cập nhật tên đầy đủ
            user.FullName = model.FullName;

            // Xử lý tải lên ảnh đại diện
            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                // Tạo thư mục lưu ảnh nếu chưa tồn tại
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Tạo tên file duy nhất
                var uniqueFileName = $"{user.Id}_{Path.GetFileName(ProfileImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh trong database
                user.ProfileImage = $"~/uploads/profiles/{uniqueFileName}";
            }

            // Lưu các thay đổi vào database
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                ViewBag.Message = "Cập nhật thành công!";
            else
                ViewBag.Message = "Cập nhật thất bại!";

            return View(user);
        }
    }
}