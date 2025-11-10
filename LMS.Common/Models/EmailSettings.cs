namespace LMS.Common.Models;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string From { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string DisplayName { get; set; } = string.Empty;
    public bool SendToActualRecipients { get; set; }
    public string OverrideRecipients { get; set; } = string.Empty;
}