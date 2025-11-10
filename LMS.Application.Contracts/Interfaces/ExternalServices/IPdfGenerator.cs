namespace LMS.Application.Contracts.Interfaces.ExternalServices;

public interface IPdfGenerator
{
    byte[] GenerateLibraryCardPdf(string templateName, Dictionary<string, string> replacements);
}