using LMS.Common.ErrorHandling;
using NToastNotify;

namespace LMS.Presentation.Configuration
{
    public static class MvcWithGlobalFiltersConfiguration
    {
        public static void AddMvcWithGlobalFilters(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddNToastNotifyToastr(new ToastrOptions
            {
                ProgressBar = true,
                PositionClass = ToastPositions.TopRight,
                PreventDuplicates = true,
                CloseButton = true,
                TimeOut = 5000,
                ExtendedTimeOut = 1000,
                ShowEasing = "swing",
                HideEasing = "linear",
                ShowMethod = "fadeIn",
                HideMethod = "fadeOut"
            });
        }
    }
}
