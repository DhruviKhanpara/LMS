namespace LMS.Common.Models;

public class EmailMessage
{
    public List<string> ToAddresses { get; set; } = new List<string>();
    public List<string>? CCAddresses { get; set; } = new List<string>();
    public List<string>? BCCAddresses { get; set; } = new List<string>();
    public string Subject { get; set; } = null!;
    public string EmailBody { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
