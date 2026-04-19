using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using DNAAnalysis.Services.Abstraction;

namespace DNAAnalysis.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
{
    try
    {
        var smtpClient = new SmtpClient(_settings.SmtpServer)
        {
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
    catch (Exception ex)
{
    // log بس
    Console.WriteLine($"Email error: {ex.Message}");

    // ما تكسرش النظام
    return;
}
}

    }
}
