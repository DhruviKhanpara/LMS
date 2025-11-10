namespace LMS.Presentation.Configuration
{
    public static class CorsConfiguration
    {
        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7184")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });
        }
    }
}
