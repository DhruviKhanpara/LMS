using LMS.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LMS.Common.Helpers;

public static class EmailHelper
{
    public static MimeMessage GenerateMessage(EmailMessage emailMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailMessage.DisplayName, emailMessage.FromAddress));
        message.Subject = emailMessage.Subject;

        var builder = new BodyBuilder { HtmlBody = emailMessage.EmailBody };
        message.Body = builder.ToMessageBody();

        message.To.AddRange(emailMessage.ToAddresses.Select(MailboxAddress.Parse));
        message.Cc?.AddRange(emailMessage.CCAddresses?.Select(MailboxAddress.Parse));
        message.Bcc?.AddRange(emailMessage.BCCAddresses?.Select(MailboxAddress.Parse));

        return message;
    }

    public static async Task<string?> SendMailAsync(EmailSettings emailSetting, MimeMessage message)
    {
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(emailSetting.Host, emailSetting.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSetting.Username, emailSetting.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return null;
        }
        catch (Exception ex)
        {
            return $"{ex.Message} {(ex.InnerException != null ? ex.InnerException : "")}";
        }
    }
}
