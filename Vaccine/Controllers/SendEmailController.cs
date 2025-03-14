using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace Vaccine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ducbao21103@gmail.com"));
            email.To.Add(MailboxAddress.Parse("phuonganhdoan.1303@gmail.com"));
            email.Subject = "Test Email";
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using var stmp = new SmtpClient();
            //stmp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            stmp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            stmp.Authenticate("ducbao21103@gmail.com", "Haducbao0938229357");
            stmp.Send(email);
            stmp.Disconnect(true);
            return Ok();

        }
    }
}
