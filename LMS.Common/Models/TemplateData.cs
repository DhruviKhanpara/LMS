namespace LMS.Common.Models;

public class TemplateData
{
    public TemplateData() { }

    public TemplateData(string templateString, Dictionary<string, string> replacements)
    {
        TemplateString = templateString;
        TemplateStringReplacement = replacements;
    }

    public string TemplateString { get; set; } = null!;
    public Dictionary<string, string> TemplateStringReplacement { get; set; } = null!;
}
