using WebApplication2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;

namespace WebApplication2.Services
{
    public class MessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        // Gửi tin nhắn
        public async Task SendMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentDate = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        // Lấy tất cả tin nhắn giữa hai người dùng
        public async Task<List<Message>> GetMessagesAsync(string userId1, string userId2)
        {
            return await _context.Messages
                                 .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                                             (m.SenderId == userId2 && m.ReceiverId == userId1))
                                 .OrderBy(m => m.SentDate)
                                 .ToListAsync();
        }
    }
}
