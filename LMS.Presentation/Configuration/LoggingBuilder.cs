using LMS.Common.Logging.Infrastructure;
using LMS.Common.Logging.Model;

namespace LMS.Presentation.Configuration;

public static class Logger
{
    public static WebApplicationBuilder BuildLogging(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("LibraryManagementSysConnection") ?? "";
        string logEventLevel = builder.Configuration["Serilog:LevelSwitches:$systemLogSwitch"] ?? "Information";

        LogBuilderRequest logBuilderRequest = new LogBuilderRequest();

        if (!string.IsNullOrEmpty(connectionString))
        {
            logBuilderRequest.LogToBuild.Add(LoggingTypes.SystemLog, new LogBuilderItem(connectionString, logEventLevel, true));
            logBuilderRequest.LogToBuild.Add(LoggingTypes.CommunicationLog, new LogBuilderItem(connectionString, logEventLevel, true));
        }

        builder.ApplyAppLogs(logBuilderRequest);

        return builder;
    }
}
