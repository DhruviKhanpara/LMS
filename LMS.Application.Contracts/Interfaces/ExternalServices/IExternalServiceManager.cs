namespace LMS.Application.Contracts.Interfaces.ExternalServices;

public interface IExternalServiceManager
{
    IPdfGenerator PdfGenerator { get; }
    IBarcodeGenerator BarcodeGenerator { get; }
    IEmailSender EmailSender { get; }
}
