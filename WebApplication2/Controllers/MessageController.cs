using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApplication2.Hubs;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;
        private readonly UserManager<Users> _userManager;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageController(
            MessageService messageService,
            UserManager<Users> userManager,
            IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        // Hiển thị trang chat với lịch sử tin nhắn
        public async Task<IActionResult> Chat(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var messages = await _messageService.GetMessagesAsync(currentUser.Id, userId);

            // Lấy thông tin người nhận để hiển thị
            var receiver = await _userManager.FindByIdAsync(userId);
            ViewBag.UserId = userId;
            ViewBag.ReceiverName = receiver?.UserName ?? "Unknown";
            ViewBag.CurrentUserId = currentUser.Id;

            return View(messages);
        }

        // Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> SendMessage(string userId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Gửi tin nhắn qua service (sẽ xử lý cả SignalR)
            await _messageService.SendMessageAsync(currentUser.Id, userId, content);

            // Nếu muốn sử dụng Ajax, trả về JSON thay vì redirect
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Chat", new { userId = userId });
        }

        // Hiển thị danh sách người dùng để chat
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Lấy danh sách tất cả người dùng trừ người dùng hiện tại
            var users = _userManager.Users.Where(u => u.Id != currentUser.Id).ToList();

            ViewBag.CurrentUserId = currentUser.Id;
            return View(users);
        }
    }
}