using System.Net.Mail;
using System.Net;

namespace WebApplication2.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var enableSsl = bool.Parse(emailSettings["EnableSsl"]);
            var fromEmail = emailSettings["FromEmail"];
            var fromName = emailSettings["FromName"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = enableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000; // 10 seconds timeout

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                try
                {
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {to}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email to {to}: {ex.Message}");
                    // Bạn có thể không ném ngoại lệ để tránh việc process bị dừng
                    return false;
                }
            }
        }
    }
}
