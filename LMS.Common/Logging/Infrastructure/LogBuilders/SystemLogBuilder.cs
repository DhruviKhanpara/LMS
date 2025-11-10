using LMS.Common.Logging.Model;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace LMS.Common.Logging.Infrastructure.LogBuilders;

public static class SystemLogBuilder
{
    public static LoggerConfiguration ApplySystemLog(this LoggerConfiguration logger, LogBuilderItem logBuilderItem)
    {
        if (logBuilderItem != null)
        {
            var sinkOptions = new MSSqlServerSinkOptions 
            { 
                TableName = LoggingTypes.SystemLog, 
                AutoCreateSqlDatabase = false,
                SchemaName = "logging"
            };

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn(LoggingProperties.UserName, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.ServerName, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.MethodType, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.Origin, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.Platform, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.Path, SqlDbType.NVarChar),
                    new SqlColumn(LoggingProperties.UserAgent, SqlDbType.NVarChar)
                }
            };

            columnOptions.Store.Remove(StandardColumn.Properties);

            logger.WriteTo.MSSqlServer(logBuilderItem.ConnectionString, sinkOptions: sinkOptions, restrictedToMinimumLevel: logBuilderItem.MinimumLogLevel, columnOptions: columnOptions);
        }
        return logger;
    }
}
