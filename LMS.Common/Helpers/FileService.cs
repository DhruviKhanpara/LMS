using LMS.Common.ErrorHandling.CustomException;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LMS.Common.Helpers;

public static class FileService
{
    public static async Task<string> FileUpload(IFormFile file, string sourceFileDirectory, string archiveDirectory, string[] allowedExtensions)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file provided.");

        if (string.IsNullOrWhiteSpace(sourceFileDirectory))
            throw new ArgumentException("Invalid file upload directory.");

        if (Path.GetInvalidFileNameChars().Any(file.FileName.Contains))
            throw new ArgumentException("File name contains invalid characters.");

        if (!ValidExtension(file: file, allowedExtensions: allowedExtensions))
            throw new BadRequestException("Ivalid file upload.");

        if (!Path.IsPathRooted(sourceFileDirectory))
        {
            string wwwRootPath = Path.GetFullPath("wwwroot");
            sourceFileDirectory = Path.Combine(path1: wwwRootPath, path2: sourceFileDirectory);
        }

        if (!Directory.Exists(path: sourceFileDirectory))
        {
            Directory.CreateDirectory(path: sourceFileDirectory);
        }

        var existingFilePath = Path.Combine(path1: sourceFileDirectory, path2: Path.GetFileNameWithoutExtension(file.FileName) + Path.GetExtension(file.FileName));

        if (File.Exists(existingFilePath))
        {
            await MoveFileToArchive(sourceFileDirectory: existingFilePath, archiveDirectory: archiveDirectory);
        }

        var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(path1: sourceFileDirectory, path2: uniqueFileName);

        try
        {
            await using (var fileStream = new FileStream(path: filePath, mode: FileMode.Create, access: FileAccess.Write, share: FileShare.None))
            {
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
            return filePath;
        }
        catch (IOException ex)
        {
            throw new Exception("An error occurred while uploading the file.", ex);
        }
    }

    public static bool ValidExtension(IFormFile file, string[] allowedExtensions)
    {
        if (file == null || allowedExtensions == null || !allowedExtensions.Any())
        {
            throw new ArgumentException("Invalid parameters provided.");
        }

        var fileExtension = Path.GetExtension(path: file.FileName).ToLower();
        return allowedExtensions.Contains(value: fileExtension);
    }

    public static async Task MoveFileToArchive(string sourceFileDirectory, string archiveDirectory)
    {
        if (string.IsNullOrWhiteSpace(sourceFileDirectory) || string.IsNullOrWhiteSpace(archiveDirectory))
            throw new ArgumentException("Source file path or archive folder cannot be null or empty.");

        if (!File.Exists(path: sourceFileDirectory))
            throw new FileNotFoundException("The source file does not exist.");


        if (!Path.IsPathRooted(archiveDirectory))
        {
            string wwwRootPath = Path.GetFullPath("wwwroot");
            archiveDirectory = Path.Combine(path1: wwwRootPath, path2: archiveDirectory);
        }

        if (!Directory.Exists(path: archiveDirectory))
        {
            Directory.CreateDirectory(path: archiveDirectory);
        }

        var destinationFilePath = Path.Combine(path1: archiveDirectory, path2: Path.GetFileName(sourceFileDirectory));
        var uniqueFilePath = destinationFilePath;

        int count = 1;
        while (File.Exists(path: uniqueFilePath))
        {
            uniqueFilePath = Path.Combine(archiveDirectory, $"{Path.GetFileNameWithoutExtension(sourceFileDirectory)}_({count}){Path.GetExtension(sourceFileDirectory)}");
            count++;
        }

        try
        {
            File.Move(sourceFileName: sourceFileDirectory, destFileName: uniqueFilePath);
        }
        catch (IOException ex)
        {
            throw new Exception("An error occurred while moving the file to the archive.", ex);
        }
    }

    public static string ConvertToRelativePath(string absolutePath)
    {
        string wwwRootPath = Path.GetFullPath("wwwroot");

        string relativePath = Path.GetRelativePath(wwwRootPath, absolutePath);

        return "/" + relativePath.Replace("\\", "/");
    }

    public static byte[] ExportMultipleSheets(Dictionary<string, IEnumerable> sheetDataMap)
    {
        if (sheetDataMap == null || sheetDataMap.Count == 0)
            throw new ArgumentException("No data provided for export.");

        IWorkbook workbook = new XSSFWorkbook();
        var dateStyle = CreateDateStyle(workbook);
        var headerStyle = CreateHeaderStyle(workbook);

        foreach (var entry in sheetDataMap)
        {
            string sheetName = SanitizeSheetName(entry.Key, workbook);
            IEnumerable data = entry.Value;

            if (data == null) continue;

            Type? itemType = data.GetType().GetGenericArguments().FirstOrDefault();
            if (itemType == null) continue;

            ISheet sheet = workbook.CreateSheet(sheetName);
            var properties = itemType.GetProperties();

            // Header row
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < properties.Length; i++)
            {
                string headerName = GetDisplayName(properties[i]);
                CreateCell(headerRow, i, headerName, headerStyle);
            }

            // Data rows
            int rowNum = 1;
            foreach (var item in data)
            {
                IRow row = sheet.CreateRow(rowNum++);
                for (int i = 0; i < properties.Length; i++)
                {
                    object? value = properties[i].GetValue(item);
                    Type actualType = Nullable.GetUnderlyingType(properties[i].PropertyType) ?? properties[i].PropertyType;

                    ICellStyle? style = null;
                    if (actualType == typeof(DateTime) || actualType == typeof(DateTimeOffset))
                        style = dateStyle;

                    CreateCell(row, i, value, style);
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        byte[] response = null;
        using (var memoryStream = new MemoryStream())
        {
            workbook.Write(memoryStream);
            response = memoryStream.ToArray();
        }
        return response;
    }

    private static void CreateCell(IRow row, int columnIndex, object? value, ICellStyle? style = null)
    {
        ICell cell = row.CreateCell(columnIndex);

        if (value == null)
        {
            cell.SetCellValue(string.Empty);
        }
        else
        {
            Type valueType = value.GetType();
            Type actualType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            if (actualType == typeof(int))
                cell.SetCellValue(Convert.ToInt32(value));
            else if (actualType == typeof(long))
                cell.SetCellValue(Convert.ToInt64(value));
            else if (actualType == typeof(float))
                cell.SetCellValue(Convert.ToDouble(value));
            else if (actualType == typeof(double))
                cell.SetCellValue(Convert.ToDouble(value));
            else if (actualType == typeof(decimal))
                cell.SetCellValue(Convert.ToDouble(value));
            else if (actualType == typeof(bool))
                cell.SetCellValue(Convert.ToBoolean(value));
            else if (actualType == typeof(DateTime))
                cell.SetCellValue(Convert.ToDateTime(value));
            else if (actualType == typeof(DateTimeOffset))
                cell.SetCellValue(((DateTimeOffset)value).DateTime);
            else
                cell.SetCellValue(value.ToString());
        }

        if (style != null)
        {
            cell.CellStyle = style;
        }
    }

    private static ICellStyle CreateHeaderStyle(IWorkbook workbook)
    {
        ICellStyle style = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        style.SetFont(font);
        return style;
    }

    private static ICellStyle CreateDateStyle(IWorkbook workbook)
    {
        ICellStyle style = workbook.CreateCellStyle();
        IDataFormat format = workbook.CreateDataFormat();
        style.DataFormat = format.GetFormat("dd-MMM-yyyy");
        return style;
    }

    private static string GetDisplayName(PropertyInfo property)
    {
        var displayAttr = property.GetCustomAttributes(typeof(DisplayAttribute), false)
                                  .FirstOrDefault() as DisplayAttribute;
        return displayAttr?.Name ?? property.Name;
    }

    private static string SanitizeSheetName(string name, IWorkbook workbook)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = "Sheet";

        // Replace invalid characters
        string sanitized = Regex.Replace(name, @"[\\/*?:\[\]]", "_");

        // Trim to 31 characters
        if (sanitized.Length > 31)
            sanitized = sanitized.Substring(0, 31);

        // Ensure uniqueness
        string baseName = sanitized;
        int suffix = 1;

        while (workbook.GetSheet(sanitized) != null)
        {
            string suffixText = "_" + suffix++;
            int maxBaseLength = 31 - suffixText.Length;

            baseName = baseName.Length > maxBaseLength ? baseName.Substring(0, maxBaseLength) : baseName;
            sanitized = baseName + suffixText;
        }

        return sanitized;
    }
}
