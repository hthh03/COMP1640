using WebApplication2.Models;
using WebApplication2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger; // Thêm ILogger để logging

        public AccountController(
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exist.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);

                    // Kiểm tra vai trò và chuyển hướng
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin"); // Chuyển hướng Admin đến Admin Dashboard
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        return RedirectToAction("Dashboard", "Teacher"); // Chuyển hướng Teacher đến Teacher Dashboard
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Student"))
                    {
                        return RedirectToAction("Dashboard", "Student"); // Chuyển hướng Student đến Student Dashboard
                    }

                    // Nếu không có vai trò cụ thể hoặc không có returnUrl hợp lệ, chuyển hướng mặc định
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Email or password is incorrect.");
            }
            return View(model);
        }

        // Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var validRoles = new List<string> { "Student", "Teacher" };
                if (!validRoles.Contains(model.Role))
                {
                    ModelState.AddModelError("", "Invalid role selection.");
                    return View(model);
                }

                // Kiểm tra Email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already registered.");
                    return View(model);
                }

                // Kiểm tra xác nhận mật khẩu
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "The password and confirmation password do not match.");
                    return View(model);
                }

                Users user = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = model.Role,
                    ProfileImage = null // Tùy chọn, có thể bỏ dòng này luôn
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} registered successfully with role: {Role}", model.Email, model.Role);

                    // Kiểm tra và tạo Role nếu chưa có
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    // Gán role cho user
                    var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }

                    // Đăng nhập tự động sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Chuyển hướng theo vai trò sau khi đăng ký
                    if (model.Role == "Teacher")
                    {
                        return RedirectToAction("Dashboard", "Teacher"); // Chuyển hướng Teacher đến Teacher Dashboard
                    }
                    else if (model.Role == "Student")
                    {
                        return RedirectToAction("Dashboard", "Student"); // Chuyển hướng Student đến Student Dashboard
                    }

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // Hiển thị trang xác minh email
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exist.");
                    return View(model);
                }

                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
            return View(model);
        }

        // Hiển thị trang đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exist.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // Đăng xuất
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}