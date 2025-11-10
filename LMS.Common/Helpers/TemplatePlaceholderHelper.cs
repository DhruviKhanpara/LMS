using LMS.Common.Models;

namespace LMS.Common.Helpers;

public static class TemplatePlaceholderHelper
{
    public static string ReplacePlaceholders(TemplateData templateData)
    {
        if (string.IsNullOrWhiteSpace(templateData.TemplateString))
        {
            throw new Exception("Unable to Retrive template");
        }

        templateData.TemplateStringReplacement
            .ToList()
            .ForEach(replacementString =>
            {
                templateData.TemplateString = templateData.TemplateString.Replace(replacementString.Key, replacementString.Value);
            });

        return templateData.TemplateString;
    }
}
