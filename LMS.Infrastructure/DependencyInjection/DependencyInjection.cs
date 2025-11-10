using DinkToPdf;
using DinkToPdf.Contracts;
using LMS.Application.Contracts.Interfaces.ExternalServices;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Common.Models;
using LMS.Infrastructure.ExternalServices;
using LMS.Infrastructure.Helper;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LMS.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LibraryManagementSysContext>(option => option.UseSqlServer(configuration.GetConnectionString("LibraryManagementSysConnection")));
        services.Configure<EmailSettings>(configuration.GetSection("SmtpConfigurations"));

        // Load native binary
        var context = new CustomAssemblyLoadContext();
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (string.IsNullOrEmpty(assemblyPath))
            throw new InvalidOperationException("Unable to resolve Infrastructure assembly directory.");

        var libPath = Path.Combine(assemblyPath, "NativeBinaries", "libwkhtmltox.dll");

        if (!File.Exists(libPath))
            throw new FileNotFoundException("Expected native library not found at: " + libPath);

        context.LoadUnmanagedLibrary(libPath);

        services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

        services.AddSingleton<IPdfGenerator, PdfGenerator>();
        services.AddSingleton<IBarcodeGenerator, BarcodeGenerator>();
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddSingleton<IExternalServiceManager, ExternalServiceManager>();

        return services;
    }
}
