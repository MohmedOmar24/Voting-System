using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Voting_System.Settings;

namespace Voting_System.Services;

public class EmailService(IOptions<MailSettings> mailsettings,
                           ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailsettings=mailsettings.Value;
    private readonly ILogger<EmailService> _logger=logger;
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailsettings.Mail),
            Subject = subject,

        };
        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder 
        {
            HtmlBody = htmlMessage
        };

        message.Body=builder.ToMessageBody();

        using var smtp= new SmtpClient();

        _logger.LogInformation("sending email to {email}", email);
        smtp.Connect(_mailsettings.Host, _mailsettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailsettings.Mail,_mailsettings.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }
}
