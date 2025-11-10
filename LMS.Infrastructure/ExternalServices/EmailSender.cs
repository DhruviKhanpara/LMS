using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.ExternalServices;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Logging.Extensions;
using LMS.Common.Logging.Model;
using LMS.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.ExternalServices;

internal class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<EmailSender> _logger;
    private readonly string _templateFolder;

    public EmailSender(IOptions<EmailSettings> options, IWebHostEnvironment webHostEnvironment, ILogger<EmailSender> logger)
    {
        _settings = options.Value;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _templateFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Templates");
    }

    public async Task SendAsync(EmailRequestDto emailRequest)
    {
        var templatePath = Path.Combine(_templateFolder, emailRequest.TemplateFileName);
        if (!File.Exists(templatePath))
            throw new Exception("Template not found");

        var templateHTML = File.ReadAllText(templatePath);

        if (emailRequest.Replacements is not null && emailRequest.Replacements.Any())
        {
            var templateData = new TemplateData(templateString: templateHTML, replacements: emailRequest.Replacements);
            templateHTML = TemplatePlaceholderHelper.ReplacePlaceholders(templateData);
        }

        // Redirect recipients if config says so
        if (!_settings.SendToActualRecipients)
        {
            if (string.IsNullOrWhiteSpace(_settings.OverrideRecipients))
                throw new InvalidOperationException("Redirect email must be set when SendToActualRecipients is false.");

            emailRequest.ToAddresses = _settings.OverrideRecipients.Split(',').Select(email => email.Trim()).ToList();
            emailRequest.CCAddresses = new List<string>();
            emailRequest.BCCAddresses = new List<string>();
        }

        var emailMessage = new EmailMessage()
        {
            FromAddress = _settings.From,
            DisplayName = _settings.DisplayName,
            ToAddresses = emailRequest.ToAddresses,
            CCAddresses = emailRequest.CCAddresses,
            BCCAddresses = emailRequest.BCCAddresses,
            Subject = emailRequest.Subject,
            EmailBody = templateHTML
        };

        EmailNotificationResult emailNotificationResult = new EmailNotificationResult(emailMessage, DateTime.UtcNow, null, null);

        try
        {
            var message = EmailHelper.GenerateMessage(emailMessage);
            var msg = await EmailHelper.SendMailAsync(emailSetting: _settings, message: message);

            if (msg is not null)
                throw new BadRequestException(msg);

            emailNotificationResult = new EmailNotificationResult(emailMessage, DateTime.UtcNow, DateTime.UtcNow, msg);

            _logger.WriteCommunication_Email(
                DeliveryStatus.Success,
                emailNotificationResult.EmailNotificationRequest.FromAddress,
                emailNotificationResult.EmailNotificationRequest.ToAddresses,
                emailNotificationResult.EmailNotificationRequest.CCAddresses,
                emailNotificationResult.EmailNotificationRequest.BCCAddresses,
                emailNotificationResult.EmailNotificationRequest.Subject,
                emailNotificationResult.EmailNotificationRequest.EmailBody,
                emailNotificationResult.ErrorMessage
            );
        }
        catch (Exception ex)
        {
            emailNotificationResult = new EmailNotificationResult(emailMessage, DateTime.UtcNow, null, $"{ex.Message} {(ex.InnerException != null ? ex.InnerException : "")}");

            _logger.WriteCommunication_Email(
                emailNotificationResult.ErrorMessage is not null ? DeliveryStatus.Success : DeliveryStatus.Failure,
                emailNotificationResult.EmailNotificationRequest.FromAddress,
                emailNotificationResult.EmailNotificationRequest.ToAddresses,
                emailNotificationResult.EmailNotificationRequest.CCAddresses,
                emailNotificationResult.EmailNotificationRequest.BCCAddresses,
                emailNotificationResult.EmailNotificationRequest.Subject,
                emailNotificationResult.EmailNotificationRequest.EmailBody,
                emailNotificationResult.ErrorMessage
            );

            throw;
        }
    }
}
