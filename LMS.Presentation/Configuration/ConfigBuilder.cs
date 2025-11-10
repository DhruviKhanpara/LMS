namespace LMS.Presentation.Configuration;

internal static class ConfigBuilder
{
    internal static WebApplicationBuilder BuilConfiguration(this WebApplicationBuilder builder)
    {
        Console.WriteLine($"Running in {builder.Environment.EnvironmentName} environment");

        builder.Configuration.AddConfiguration(
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                    "appsettings.json", 
                    optional: false, 
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json", 
                    optional: true, 
                    reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build()
        );

        return builder;
    }
}
