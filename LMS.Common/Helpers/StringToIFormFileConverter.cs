using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace LMS.Common.Helpers;

public class StringToIFormFileConverter : ITypeConverter<string?, IFormFile?>
{
    public IFormFile? Convert(string? source, IFormFile? destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source) || !File.Exists(source))
            return null;

        var fileStream = new FileStream(source, FileMode.Open, FileAccess.Read);
        return new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(source))
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };
    }
}
