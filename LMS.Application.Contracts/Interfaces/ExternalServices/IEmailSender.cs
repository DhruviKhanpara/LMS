using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.ExternalServices;

public interface IEmailSender
{
    Task SendAsync(EmailRequestDto emailRequest);
}
