namespace LMS.Common.Logging.Model;

public static class LoggingTypes
{
    public const string SystemLog = "SystemLog";
    public const string AppliactionLog = "ApplicationLog";
    public const string CommunicationLog = "CommunicationLog";
    public const string AuditLog = "AuditLog";

    public static string[] AllNonSystemLoggingTypes = new[] { AppliactionLog, AuditLog, CommunicationLog };
}
