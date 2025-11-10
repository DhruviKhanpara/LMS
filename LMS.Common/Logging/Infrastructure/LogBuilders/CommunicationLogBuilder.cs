using LMS.Common.Logging.Model;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using System.Collections.ObjectModel;
using System.Data;
using Serilog.Filters;

namespace LMS.Common.Logging.Infrastructure.LogBuilders;

public static class CommunicationLogBuilder
{
    public static LoggerConfiguration ApplyCommunicationLog(this LoggerConfiguration logger, LogBuilderItem logBuilderItem)
    {
        if (logBuilderItem != null)
        {
            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = LoggingTypes.CommunicationLog,
                AutoCreateSqlDatabase = false,
                SchemaName = "logging"
            };

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn(LoggingProperties.UserName, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.DeliveryMethod, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.DeliveryStatus, SqlDbType.NVarChar),
                }
            };

            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.Store.Remove(StandardColumn.Exception);
            columnOptions.LogEvent.ExcludeStandardColumns = true;

            logger
                .Filter.ByIncludingOnly(Matching.WithProperty(LoggingProperties.LoggingType, LoggingTypes.CommunicationLog))
                .WriteTo.MSSqlServer(logBuilderItem.ConnectionString, sinkOptions: sinkOptions, columnOptions: columnOptions);
        }
        return logger;
    }
}
