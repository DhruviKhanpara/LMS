namespace LMS.Application.Contracts.DTOs;

public class EmailRequestDto
{
    public string TemplateFileName { get; set; } = null!;
    public Dictionary<string, string>? Replacements { get; set; } = new Dictionary<string, string>();
    public string Subject { get; set; } = null!;
    public List<string> ToAddresses { get; set; } = new List<string>();
    public List<string>? CCAddresses { get; set; } = new List<string>();
    public List<string>? BCCAddresses { get; set; } = new List<string>();

    public EmailRequestDto(string templateFileName, string subject, List<string> toAddresses, Dictionary<string, string>? replacements = null, List<string>? ccAddresses = null, List<string>? bccAddresses = null)
    {
        TemplateFileName = templateFileName;
        Subject = subject;
        ToAddresses = toAddresses;
        CCAddresses = ccAddresses ?? new List<string>();
        BCCAddresses = bccAddresses ?? new List<string>();
        Replacements = replacements ?? new Dictionary<string, string>();
    }
}
