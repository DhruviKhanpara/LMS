using DinkToPdf;
using DinkToPdf.Contracts;
using LMS.Application.Contracts.Interfaces.ExternalServices;
using LMS.Common.Helpers;
using LMS.Common.Models;
using Microsoft.AspNetCore.Hosting;

namespace LMS.Infrastructure.ExternalServices;

internal class PdfGenerator : IPdfGenerator
{
    private readonly IConverter _converter;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _templateFolder;

    public PdfGenerator(IConverter converter, IWebHostEnvironment webHostEnvironment)
    {
        _converter = converter;
        _webHostEnvironment = webHostEnvironment;
        _templateFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Templates");
    }

    public byte[] GenerateLibraryCardPdf(string templateName, Dictionary<string, string> replacements)
    {
        var templatePath = Path.Combine(_templateFolder, templateName);
        if (!File.Exists(templatePath))
            throw new Exception("Template not found");

        var templateHTML = File.ReadAllText(templatePath);

        var templateData = new TemplateData(templateString: templateHTML, replacements: replacements);
        templateHTML = TemplatePlaceholderHelper.ReplacePlaceholders(templateData);

        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
            Objects = {
                new ObjectSettings {
                    HtmlContent = templateHTML,
                    WebSettings = new WebSettings
                    {
                        LoadImages = true,
                        EnableJavascript = true,
                        EnableIntelligentShrinking = true,
                        DefaultEncoding = "utf-8",
                    }
                }
            }
        };

        return _converter.Convert(doc);
    }
}
