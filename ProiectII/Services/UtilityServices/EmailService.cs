using Microsoft.Extensions.Logging;
using ProiectII.Interfaces;


namespace ProiectII.Services.UtilityServices

{

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        Task<bool> IEmailService.SendEmailAsync(string toEmail, string subject, string body)
                    {
            _logger.LogInformation("================================================================");
            _logger.LogInformation("OUTGOING EMAIL SYSTEM | Status: SIMULATED_SEND");
            _logger.LogInformation("To: {Email}", toEmail);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Content: {Body}", body);
            _logger.LogInformation("================================================================");

            return (Task<bool>)Task.CompletedTask;
        }
    }



}