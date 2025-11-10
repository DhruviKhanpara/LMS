using LMS.Common.Models;

namespace LMS.Common.Helpers;

public static class LogTablesColorHelper
{
    private static readonly Dictionary<string, (string ForeColor, string BgColor)> _colors = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { nameof(LogOperationTypes.Insert),     ("#4CAF50", "#4CAF5029") },
        { nameof(LogOperationTypes.Update),     ("#2196F3", "#2196F329") },
        { nameof(LogOperationTypes.SoftDelete), ("#FF9800", "#FF980029") },
        { nameof(LogOperationTypes.Restore),    ("#9C27B0", "#9C27B029") },
        { nameof(LogOperationTypes.Delete),     ("#F44336", "#F4433629") },
        { nameof(LogCategory.Information),         ("#03A9F4", "#03A9F429") },
        { nameof(LogCategory.Warning),             ("#FFC107", "#FFC10729") },
        { nameof(LogCategory.Error),               ("#E53935", "#E5393529") }
    };

    public static string GetOperationTypeForeColor(string? logOpeartion) =>
        logOpeartion != null && _colors.TryGetValue(logOpeartion, out var color) ? color.ForeColor : "#6C757D";

    public static string GetOperationTypeBgColor(string? logOpeartion) =>
        logOpeartion != null && _colors.TryGetValue(logOpeartion, out var color) ? color.BgColor : "#6C757D29";

    public static string GetLogTypeForeColor(string? logType) =>
        logType != null && _colors.TryGetValue(logType, out var color) ? color.ForeColor : "#6C757D";

    public static string GetLogTypeBgColor(string? logType) =>
        logType != null && _colors.TryGetValue(logType, out var color) ? color.BgColor : "#6C757D29";
}

public static class PerformByHelper
{
    public static string? GetPerformBy(string? operation, string? createdBy, string? modifiedBy, string? deletedBy)
    {
        return operation switch
        {
            LogOperationTypes.Insert => createdBy,
            LogOperationTypes.Update or 
            LogOperationTypes.Restore => modifiedBy,
            LogOperationTypes.Delete or 
            LogOperationTypes.SoftDelete => deletedBy,
            _ => null,
        };
    }
}