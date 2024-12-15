namespace taskQueueGrupo3.Services
{
    public interface IEmailService
    {
        System.Threading.Tasks.Task SendEmailAsync(string toEmail, string subject, string message);
    }
}