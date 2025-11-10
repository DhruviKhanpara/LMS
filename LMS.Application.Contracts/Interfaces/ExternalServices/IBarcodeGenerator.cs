namespace LMS.Application.Contracts.Interfaces.ExternalServices;

public interface IBarcodeGenerator
{
    byte[] GenerateQrCode(string content);
    byte[] GenerateBarcode(string content);
}
