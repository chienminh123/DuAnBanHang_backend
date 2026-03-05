using System.Net;
using System.Net.Mail;

namespace backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config,IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var mail = "smtp.gmail.com";
            var pw = emailSettings["Password"];
            var from = emailSettings["Email"];

            var client = new SmtpClient(mail, 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(from, pw)
            };

            var mailMessage = new MailMessage(from: from, to: toEmail, subject, message);
            mailMessage.IsBodyHtml = true;
            // Gửi mail
            await client.SendMailAsync(mailMessage);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string tenNhanVien, string tenTaiKhoan, string matKhau)
        {
            string filePath = Path.Combine(_env.WebRootPath, "templates", "WelcomeEmployee.html");
            string body = await File.ReadAllTextAsync(filePath);

            body = body.Replace("{{TenNhanVien}}", tenNhanVien)
                       .Replace("{{TenTaiKhoan}}", tenTaiKhoan)
                       .Replace("{{MatKhau}}", matKhau);

            await SendEmailAsync(toEmail, "[Hệ thống] Thông tin tài khoản nhân viên mới", body);
        }
    }
}