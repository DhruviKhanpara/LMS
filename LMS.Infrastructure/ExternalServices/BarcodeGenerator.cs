using LMS.Application.Contracts.Interfaces.ExternalServices;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace LMS.Infrastructure.ExternalServices;

internal class BarcodeGenerator : IBarcodeGenerator
{
    public byte[] GenerateQrCode(string content)
    {
        byte[] byteArray;
        var width = 150;
        var height = 150;
        var margin = 0;
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODABAR,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                Margin = margin
            }
        };
        var pixelData = qrCodeWriter.Write(content);

        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        {
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byteArray = ms.ToArray();
            }
            return byteArray;
        }
    }

    public byte[] GenerateBarcode(string content)
    {
        byte[] byteArray;
        var width = 80;
        var height = 30;
        var margin = 1;

        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = height,
                Width = width,
                Margin = margin
            }
        };

        var pixelData = writer.Write(content);

        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        {
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byteArray = ms.ToArray();
            }
            return byteArray;
        }
    }
}
