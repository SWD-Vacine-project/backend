namespace Vaccine.API.Controllers
{
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;
    using System.Net.Mail;
    using Vaccine.API.Models.HolidayModel;
    using Vaccine.Repo.Entities;
    using Vaccine.Repo.UnitOfWork;

    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public EmailController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("thunm300104@gmail.com");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("thunm300104@gmail.com", "pyzy ogej ysgq criv");
            smtpClient.EnableSsl = true;

            smtpClient.Send(mail);
        }

        [HttpGet("testEmail")]
        public async Task<IActionResult> TestEmail()
        {
            SendEmail("ngminhthu3001@gmail.com", "Xin Chào", "Xin Chào");

            return Ok();
        }

    }
}
