using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApplication2.ViewModels;

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
        public async Task<IActionResult> Profile(Users model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            user.FullName = model.FullName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                TempData["SuccessMessage"] = "Profile Updated Successfully!";
            else
                ViewBag.Message = "Update Failed!";

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile ProfileImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    string oldImagePath = user.ProfileImage.Replace("~/", "");
                    oldImagePath = oldImagePath.TrimStart('/');
                    string fullOldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath);

                    if (System.IO.File.Exists(fullOldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(fullOldImagePath);
                        }
                        catch (IOException ex)
                        {
                        }
                    }
                }

                var uniqueFileName = $"{user.Id}_{Path.GetFileName(ProfileImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(fileStream);
                }

                user.ProfileImage = $"~/uploads/profiles/{uniqueFileName}";

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return Json(new { success = true, message = "Image Updated Successfully!", imagePath = user.ProfileImage });
                else
                    return Json(new { success = false, message = "Failed To Update Profile Image" });
            }

            return Json(new { success = false, message = "No Image Was Uploaded" });
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data received" });
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.Remove("Email");
            }

            if (string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                ModelState.Remove("ConfirmNewPassword");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model data" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user,
                model.CurrentPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errorMessages = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errorMessages });
            }

            await _signInManager.RefreshSignInAsync(user);
            return Json(new { success = true, message = "Password has been changed successfully!" });
        }
    }
}