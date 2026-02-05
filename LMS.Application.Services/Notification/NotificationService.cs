using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Services.Constants;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Tls;
using System.Globalization;
using System.Text;

namespace LMS.Application.Services.Notification;

public class NotificationService : INotificationService
{
    private const string GENERIC_EMAIL_TEMPLATE = "GenericMailTemplate.html";

    private readonly INotificationDispatcher _dispatcher;
    private readonly IRepositoryManager _repositoryManager;
    private readonly string _baseUrl;

    public NotificationService(INotificationDispatcher dispatcher, IRepositoryManager repositoryManager)
    {
        _dispatcher = dispatcher;
        _repositoryManager = repositoryManager;
        _baseUrl = "https://localhost:7184";
    }

    public async Task DispatchEmail(NotificationTypeEnum type, object? data)
    {
        var emailRequest = await GenerateEmailRequest(type, data);
        await DispatchEmailToDB(emailRequest);
    }

    public async Task DispatchMultipleEmail(NotificationTypeEnum type, List<object> dataList)
    {
        List<EmailRequestDto> emailRequest = new List<EmailRequestDto>();

        foreach (var item in dataList)
        {
            emailRequest.Add(await GenerateEmailRequest(type, item));
        }

        await DispatchEmailToDB(emailRequest);
    }

    private async Task<EmailRequestDto> GenerateEmailRequest(NotificationTypeEnum type, object? data)
    {
        var recipients = await GetRecipientsByNotificationType(type, data);
        var ccAddresses = await GetCcAddressesByNotificationType(type);
        var bccAddresses = await GetBccAddressesByNotificationType(type);
        var templateName = await GetTemplateNameByNotificationType(type);
        (string subject, Dictionary<string, string> replacements) = await GetTemplateInfoByNotificationType(type, data);

        return new EmailRequestDto(templateName, subject, recipients, replacements, ccAddresses, bccAddresses);
    }

    private async Task DispatchEmailToDB(EmailRequestDto emailRequest)
    {
        await _dispatcher.DispatchEmailAsync(emailRequest);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task DispatchEmailToDB(List<EmailRequestDto> emailRequests)
    {
        await _dispatcher.DispatchEmailAsync(emailRequests);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    private async Task<List<string>> GetRecipientsByNotificationType(NotificationTypeEnum type, object? data)
    {
        var emailRecipients = type switch
        {
            NotificationTypeEnum.NewCheckout or
            NotificationTypeEnum.OverdueCheckout or
            NotificationTypeEnum.RenewCheckout or
            NotificationTypeEnum.NewMembership or
            NotificationTypeEnum.UpgradeMembership or
            NotificationTypeEnum.MembershipDue => GetRecipientsFromUserObjectInData(data),
            NotificationTypeEnum.DueDateRemainder or
            NotificationTypeEnum.ReservationAllocation  => GetRecipientsFromUserObjectInDataOfTypeList(data),
            _ => default
        };

        if (emailRecipients is null || !emailRecipients.Any(x => !string.IsNullOrEmpty(x)))
            throw new NotFoundException($"Email type {type} not configured");

        return await Task.FromResult(emailRecipients);
    }

    private async Task<List<string>> GetCcAddressesByNotificationType(NotificationTypeEnum type)
    {
        var ccAddresses = type switch
        {
            _ => new List<string>()
        };

        return await Task.FromResult(ccAddresses);
    }

    private async Task<List<string>> GetBccAddressesByNotificationType(NotificationTypeEnum type)
    {
        var bccAddresses = type switch
        {
            _ => new List<string>()
        };

        return await Task.FromResult(bccAddresses);
    }

    private async Task<string> GetTemplateNameByNotificationType(NotificationTypeEnum type)
    {
        var templateName = type switch
        {
            _ => GENERIC_EMAIL_TEMPLATE
        };

        return await Task.FromResult(templateName);
    }

    private async Task<(string, Dictionary<string, string>)> GetTemplateInfoByNotificationType(NotificationTypeEnum type, object? data)
    {
        var (subject, replacements) = type switch
        {
            NotificationTypeEnum.NewCheckout => GetNewCheckoutTemplateInfo(data),
            NotificationTypeEnum.OverdueCheckout => GetOverdueCheckoutTemplateInfo(data),
            NotificationTypeEnum.RenewCheckout => GetRenewCheckoutTemplateInfo(data),
            NotificationTypeEnum.DueDateRemainder => GetDueDateRemainderTemplateInfo(data),
            NotificationTypeEnum.ReservationAllocation => await GetReservationAllocationTemplateInfo(data),
            NotificationTypeEnum.NewMembership => GetNewMembershipTemplateInfo(data),
            NotificationTypeEnum.UpgradeMembership => GetUpgradeMembershipTemplateInfo(data),
            NotificationTypeEnum.MembershipDue => GetMembershipDueTemplateInfo(data),
            _ => throw new NotFoundException($"{type} type notification not configured")
        };

        return await Task.FromResult<(string, Dictionary<string, string>)>((subject, replacements));
    }

    private List<string> GetRecipientsFromUserObjectInData(object? data)
    {
        var userMail = data?.TryGetDynamicPropertyValue<string>("User.Email");

        var recipients = new List<string>();
        if (!string.IsNullOrEmpty(userMail))
            recipients.Add(userMail);

        return recipients;
    }

    private List<string> GetRecipientsFromUserObjectInDataOfTypeList(object? data)
    {
        if (data is not IEnumerable<object> dataList || !dataList.Any())
            throw new BadRequestException("No recipients are found");

        var userMail = dataList.FirstOrDefault()?.TryGetDynamicPropertyValue<string>("User.Email");

        var recipients = new List<string>();
        if (!string.IsNullOrEmpty(userMail))
            recipients.Add(userMail);

        return recipients;
    }

    #region NewCheckout

    private (string, Dictionary<string, string>) GetNewCheckoutTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var bookName = data?.TryGetDynamicPropertyValue<string>("Book.Title");
        var statusId = data?.TryGetDynamicPropertyValue<long>("StatusId");
        var status = data?.TryGetDynamicPropertyValue<string>("Status.Label");
        var borrowDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("BorrowDate");
        var dueDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("DueDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>

            <p style="margin-bottom: 10px;">This email is to inform you about a recent update to your library account. Below are the details of the new check-outs:</p>
            
            <p style="margin-bottom: 10px;">
                <strong>Book Title</strong>         : {{bookName}}<br>
                <strong>Transaction Type</strong>   : {{status}}<br>
                <strong>Borrow Date</strong>        : {{(borrowDate.HasValue ? borrowDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
        """
        ;

        if (statusId == (long)TransectionStatusEnum.Renewed)
        {
            var renewDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("RenewDate");
            htmlBody += $"<strong>Renewal Date</strong>       : {(renewDate.HasValue ? renewDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}<br>";
        }

        htmlBody += $$"""
                <strong>Due Date</strong>           : {{(dueDate.HasValue ? dueDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}
            </p>
            
            <p style="margin-bottom: 10px;">Please remember to return the book on or before the due date to avoid any late fees. You can return the book to the library during our operating hours.</p>
            
            <p style="margin-bottom: 10px;">
                <strong>Note:</strong> If this transaction was not directly performed by you, it may have been handled by library staff to ensure your account reflects the latest updates accurately.
            </p>

            <p style="margin-bottom: 10px;">For any questions or assistance, feel free to contact us or visit the library website.</p>
            
            <p style="margin-bottom: 10px;">Thank you for being a valued library member.</p>
            
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """
        ;

        string subject = $"Book Borrow Confirmation and Details - [{bookName}]";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/transection/get-user-transection" }
        };

        return (subject, replacements);
    }

    #endregion

    #region OverdueCheckout

    private (string, Dictionary<string, string>) GetOverdueCheckoutTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var bookName = data?.TryGetDynamicPropertyValue<string>("Book.Title");
        var borrowDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("BorrowDate");
        var dueDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("DueDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>
        
            <p style="margin-bottom: 10px;">We hope you're enjoying your reading experience. This is a friendly reminder that the following item from your library account is now overdue:</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Book Title</strong>       : {{bookName}}<br>
                <strong>Borrow Date</strong>      : {{(borrowDate.HasValue ? borrowDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Due Date</strong>         : {{(dueDate.HasValue ? dueDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Days Overdue</strong>     : {{(dueDate.HasValue ? (DateTimeOffset.UtcNow.Date - dueDate.Value.Date).Days : 0)}}
            </p>
        
            <p style="margin-bottom: 10px;">Please return the book as soon as possible to avoid further late fees or account restrictions. You can return it during our regular operating hours at the library front desk.</p>
        
            <p style="margin-bottom: 10px;">If you've already returned the book, kindly disregard this message. Otherwise, we appreciate your prompt attention to this matter.</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Note:</strong> Continued overdue status may result in suspension of borrowing privileges or additional fines as per library policy.
            </p>
        
            <p style="margin-bottom: 10px;">For any questions or assistance, feel free to contact us or visit the library website.</p>
        
            <p style="margin-bottom: 10px;">Thank you for being a valued library member.</p>
        
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """;

        string subject = $"Avoid Late Fees—Return Your Overdue Book ASAP";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/transection/get-user-transection" }
        };

        return (subject, replacements);
    }

    #endregion

    #region RenewCheckout

    private (string, Dictionary<string, string>) GetRenewCheckoutTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var bookName = data?.TryGetDynamicPropertyValue<string>("Book.Title");
        var borrowDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("BorrowDate");
        var dueDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("DueDate");
        var renewDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("RenewDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>
        
            <p style="margin-bottom: 10px;">Good news! Your checkout for the following item has been successfully renewed:</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Book Title</strong>       : {{bookName}}<br>
                <strong>Transaction Type</strong> : Renew<br>
                <strong>Borrow Date</strong>      : {{(borrowDate.HasValue ? borrowDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Renewal Date</strong>     : {{(renewDate.HasValue ? renewDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Due Date</strong>         : {{(dueDate.HasValue ? dueDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}
            </p>
        
            <p style="margin-bottom: 10px;">You can continue enjoying your reading until the new due date. Please ensure the item is returned or renewed again before this date to avoid any late fees.</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Note:</strong> You can renew the book multiple times, subject to our library's policy limit. You can check the renewal limit on <a href="{{_baseUrl}}/rules/get-instructions">our website</a> or contact the librarian.
            </p>
        
            <p style="margin-bottom: 10px;">For any questions or assistance, feel free to contact us or visit the library website.</p>
        
            <p style="margin-bottom: 10px;">Thank you for being a valued library member — we’re glad to keep you reading!</p>
        
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """;

        string subject = $"You're All Set—Your Book's Been Renewed!";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/transection/get-user-transection" }
        };

        return (subject, replacements);
    }

    #endregion

    #region DueDateRemainder

    private (string, Dictionary<string, string>) GetDueDateRemainderTemplateInfo(object? data)
    {
        if (data != null && data is List<Transection> transections && transections.Any())
        {
            var userFirstName = transections.First().User?.FirstName;
            var userMiddleName = transections.First().User?.MiddleName;
            var userLastName = transections.First().User?.LastName;

            var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

            var bookRows = new StringBuilder();

            foreach (var transaction in transections)
            {
                var bookName = transaction.Book?.Title ?? "Unknown Title";
                var dueDate = transaction.DueDate;
                var remainingDays = (transaction.DueDate.Date - DateTimeOffset.UtcNow.Date).Days;

                bookRows.AppendLine($"<tr><td>{bookName}</td><td>{dueDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}</td><td>{remainingDays}</td></tr>");
            }

            var htmlBody = $$"""
                <p style="margin-bottom: 10px;">Dear {{userName}},</p>
            
                <p style="margin-bottom: 10px;">Just a friendly reminder that the following items you borrowed from the library are due soon:</p>
            
                <table border="1" cellpadding="6" cellspacing="0" style="border-collapse: collapse; margin-bottom: 15px; font-family: Arial, sans-serif; font-size: 14px;">
                    <thead style="background-color: #f2f2f2;">
                        <tr>
                            <th>Book Name</th>
                            <th>Due Date</th>
                            <th>Remaining Days</th>
                        </tr>
                    </thead>
                    <tbody>
                        {{bookRows}}
                    </tbody>
                </table>
            
                <p style="margin-bottom: 10px;">To avoid late fees or account restrictions, please return or renew the book by the due date. You can renew online through your library account or visit us in person.</p>
            
                <p style="margin-bottom: 10px;">
                    <strong>Note:</strong> Renewals are quick and easy—just log in to your account or contact our staff.
                </p>
            
                <p style="margin-bottom: 10px;">We’re here to assist! Reach out to our support team or visit the library website for more information.</p>
            
                <p style="margin-bottom: 10px;">Thank you for being a valued library member.</p>
            
                <p style="margin-bottom: 10px;">Sincerely,<br>
                Library Team</p>
            """;

            string subject = $"Your Book Is Almost Due—Just Check Your Due Date First";

            Dictionary<string, string> replacements = new Dictionary<string, string>()
            {
                { "{{Mail text}}", htmlBody },
                { "{{link}}", $"{_baseUrl}/transection/get-user-transection" }
            };

            return (subject, replacements);
        }
        else
        {
            throw new BadRequestException("Transaction data is not in valid form");
        }
    }

    #endregion

    #region ReservationAllocation

    private async Task<(string, Dictionary<string, string>)> GetReservationAllocationTemplateInfo(object? data)
    {
        long.TryParse((await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync(ConfigKeysConstants.AllocationDueDays)
                .FirstOrDefaultAsync())?.KeyValue ?? "0", out long allocationDueDays);

        if (data != null && data is List<Reservation> reservations && reservations.Any())
        {
            if (reservations.Count > 1)
            {
                var userFirstName = reservations.First().User?.FirstName;
                var userMiddleName = reservations.First().User?.MiddleName;
                var userLastName = reservations.First().User?.LastName;

                var userName = $"{userFirstName} {userMiddleName} {userLastName}".Trim();

                var bookRows = new StringBuilder();

                foreach (var reservation in reservations)
                {
                    var bookName = reservation.Book?.Title ?? "Unknown Title";
                    var allocatedDate = reservation.AllocatedAt;
                    var pickupDeadline = reservation.AllocatedAt.HasValue ? reservation.AllocatedAt.Value.AddDays(allocationDueDays) : DateTimeOffset.Now;

                    var isExpiringSoon = pickupDeadline.Date == DateTimeOffset.Now.Date.AddDays(1);
                    var rowStyle = isExpiringSoon ? "style='background-color:#fff3cd;'" : "";

                    bookRows.AppendLine($"""
                        <tr {rowStyle}>
                            <td>{bookName}</td>
                            <td>{(allocatedDate.HasValue ? allocatedDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}</td>
                            <td>{pickupDeadline.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}</td>
                        </tr>
                    """);
                }

                var htmlBody = $$"""
                    <p style="margin-bottom: 10px;">Dear {{userName}},</p>
                
                    <p style="margin-bottom: 10px;">
                        Good news! You’ve got book{{(reservations.Count > 1 ? "s" : "")}} waiting for pickup—some newly allocated, and some close to their deadline. Check the list below and swing by the librarysoon!
                    </p>
                
                    <table border="1" cellpadding="6" cellspacing="0" style="border-collapse: collapse; margin-bottom: 15px; font-family: Arial, sans-serif; font-size: 14px;">
                        <thead style="background-color: #f2f2f2;">
                            <tr>
                                <th>Book Title</th>
                                <th>Allocated Date</th>
                                <th>Pickup Deadline</th>
                            </tr>
                        </thead>
                        <tbody>
                            {{bookRows}}
                        </tbody>
                    </table>
                
                    <p style="margin-bottom: 10px;">
                        Please visit the library before the pickup deadline to collect your book{{(reservations.Count > 1 ? "s" : "")}}. If you’re unable to make it, contact us to extend or reschedule your pickup.
                    </p>
                
                    <p style="margin-bottom: 10px;">Thank you for using our reservation service.</p>
                
                    <p style="margin-bottom: 10px;">Sincerely,<br>
                    Library Team</p>
                """;

                string subject = $"Your Reserved Books Are Waiting—Don’t Miss the Deadline!";

                Dictionary<string, string> replacements = new Dictionary<string, string>()
                {
                    { "{{Mail text}}", htmlBody },
                    { "{{link}}", $"{_baseUrl}/reservation/get-user-reservation" }
                };

                return (subject, replacements);
            }
            else
            {
                var reservation = reservations.First();

                var userFirstName = reservation.User?.FirstName;
                var userMiddleName = reservation.User?.MiddleName;
                var userLastName = reservation.User?.LastName;
                var bookName = reservation.Book?.Title ?? "Unknown Title";
                var allocatedDate = reservation.AllocatedAt;
                var pickupDeadline = reservation.AllocatedAt.HasValue ? reservation.AllocatedAt.Value.AddDays(allocationDueDays) : DateTimeOffset.Now;

                var userName = $"{userFirstName} {userMiddleName} {userLastName}".Trim();

                var htmlBody = $$"""
                    <p style="margin-bottom: 10px;">Dear {{userName}},</p>
                
                    <p style="margin-bottom: 10px;">
                        Good news! You’ve got a book waiting for pickup—newly allocated and ready for you. Check the details below and swing by the library soon!
                    </p>
                
                    <p style="margin-bottom: 10px;">
                        <strong>Book Title</strong>      : {{bookName}}<br>
                        <strong>Allocated Date</strong>  : {{(allocatedDate.HasValue ? allocatedDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                        <strong>Pickup Deadline</strong> : {{pickupDeadline.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}}
                    </p>
                
                    <p style="margin-bottom: 10px;">
                        Please visit the library before the pickup deadline to collect your book. If you’re unable to make it, contact us to extend or reschedule your pickup.
                    </p>
                
                    <p style="margin-bottom: 10px;">Thank you for using our reservation service.</p>
                
                    <p style="margin-bottom: 10px;">Sincerely,<br>
                    Library Team</p>
                """;

                string subject = $"Your Reserved Books Are Waiting—Don’t Miss the Deadline!";

                Dictionary<string, string> replacements = new Dictionary<string, string>()
                {
                    { "{{Mail text}}", htmlBody },
                    { "{{link}}", $"{_baseUrl}/reservation/get-user-reservation" }
                };

                return (subject, replacements);
            }
            
        }
        else
        {
            throw new BadRequestException("Reservation data is not in valid form");
        }
    }

    #endregion

    #region NewMembership

    private (string, Dictionary<string, string>) GetNewMembershipTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var membershipType = data?.TryGetDynamicPropertyValue<string>("Membership.Type");
        var startDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("EffectiveStartDate");
        var expiryDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("ExpirationDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>
        
            <p style="margin-bottom: 10px;">Welcome to the library! We're thrilled to confirm your new membership:</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Membership Type</strong> : {{membershipType}}<br>
                <strong>Start Date</strong>      : {{(startDate.HasValue ? startDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Expiry Date</strong>     : {{(expiryDate.HasValue ? expiryDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}
            </p>
        
            <p style="margin-bottom: 10px;">With your membership, you now have access to:</p>
        
            <ul style="margin-bottom: 10px;">
                <li>Book borrowing privileges</li>
                <li>Reservation and renewal services</li>
            </ul>
        
            <p style="margin-bottom: 10px;">You can manage your account, track your check-outs, and explore our catalog online anytime.</p>
        
            <p style="margin-bottom: 10px;">If you have any questions or need assistance, feel free to contact us or visit the library desk.</p>
        
            <p style="margin-bottom: 10px;">Thank you for joining our library community—we’re excited to have you with us!</p>
        
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """;

        string subject = $"Welcome to the Library—Your Membership Is Now Active";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/transection/get-transection-by-user" }
        };

        return (subject, replacements);
    }

    #endregion

    #region NewMembership

    private (string, Dictionary<string, string>) GetUpgradeMembershipTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var newMembershipType = data?.TryGetDynamicPropertyValue<string>("NewMembershipType");
        var oldMembershipType = data?.TryGetDynamicPropertyValue<string>("OldMembershipType");
        var startDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("EffectiveStartDate");
        var expiryDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("ExpirationDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>
        
            <p style="margin-bottom: 10px;">We’re happy to let you know that your membership has been successfully upgraded!</p>
        
            <p style="margin-bottom: 10px;">
                <strong>Previous Membership</strong> : {{oldMembershipType}}<br>
                <strong>New Membership</strong>      : {{newMembershipType}}<br>
                <strong>Start Date</strong>          : {{(startDate.HasValue ? startDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Expiry Date</strong>         : {{(expiryDate.HasValue ? expiryDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}
            </p>
        
            <p style="margin-bottom: 10px;">Your upgraded membership includes:</p>
        
            <ul style="margin-bottom: 10px;">
                <li>Enhanced book borrowing privileges</li>
                <li>Priority reservation and renewal services</li>
                <li>Access to exclusive library resources</li>
            </ul>
        
            <p style="margin-bottom: 10px;">You can manage your account, track your check-outs, and explore our catalog online anytime.</p>
        
            <p style="margin-bottom: 10px;">If you have any questions or need assistance, feel free to contact us or visit the library desk.</p>
        
            <p style="margin-bottom: 10px;">Thank you for continuing your journey with us—we’re delighted to support your reading adventures!</p>
        
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """;

        string subject = $"Welcome to the Library—Your Membership Is Now Active";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/transection/get-transection-by-user" }
        };

        return (subject, replacements);
    }

    #endregion

    #region MembershipDue

    private (string, Dictionary<string, string>) GetMembershipDueTemplateInfo(object? data)
    {
        var userFirstName = data?.TryGetDynamicPropertyValue<string>("User.FirstName");
        var userMiddleName = data?.TryGetDynamicPropertyValue<string>("User.MiddleName");
        var userLastName = data?.TryGetDynamicPropertyValue<string>("User.LastName");
        var membershipType = data?.TryGetDynamicPropertyValue<string>("Membership.Type");
        var expiryDate = data?.TryGetDynamicPropertyValue<DateTimeOffset>("ExpirationDate");

        var userName = $"{userFirstName} {userMiddleName ?? ""} {userLastName ?? ""}".Trim();

        var htmlBody = $$"""
            <p style="margin-bottom: 10px;">Dear {{userName}},</p>
        
            <p style="margin-bottom: 10px;">
                This is a friendly reminder that your <strong>{{membershipType}}</strong> is set to expire soon:
            </p>
        
            <p style="margin-bottom: 10px;">
                <strong>Expiry Date</strong>     : {{(expiryDate.HasValue ? expiryDate.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "")}}<br>
                <strong>Days Remaining</strong>  : {{(expiryDate.HasValue ? (expiryDate.Value.Date - DateTimeOffset.UtcNow.Date).Days : 0)}} days
            </p>
        
            <p style="margin-bottom: 10px;">
                To continue enjoying uninterrupted access to our library services, please renew your membership before the expiry date.
            </p>
        
            <p style="margin-bottom: 10px;">
                If you currently have books checked out, we strongly recommend renewing your membership promptly to avoid late penalties. Also, please review your current number of check-outs and choose a membership plan that suits your borrowing needs—exceeding your limit may result in additional holding fees.
            </p>
        
            <p style="margin-bottom: 10px;">
                You can renew online through your account (<a href="{{_baseUrl}}/membership/get-all-plan">LMS</a>) or visit the library desk for assistance.
            </p>
        
            <p style="margin-bottom: 10px;">
                If you've already renewed, kindly disregard this message.
            </p>
        
            <p style="margin-bottom: 10px;">Thank you for being a valued member of our library community.</p>
        
            <p style="margin-bottom: 10px;">Sincerely,<br>
            Library Team</p>
        """;

        string subject = $"Your Library Membership Is About to Expire—Renew Today";

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{Mail text}}", htmlBody },
            { "{{link}}", $"{_baseUrl}/membership/get-all-plan" }
        };

        return (subject, replacements);
    }

    #endregion
}
