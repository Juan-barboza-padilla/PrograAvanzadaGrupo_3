using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using taskQueueGrupo3.Models;

namespace taskQueueGrupo3.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            })
            {
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}