using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace LMS.Presentation.Configuration
{
    public static class AuthenticationConfiguration
    {
        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
          {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:SecretKey").Value!)),
                    ValidateAudience = true,
                    ValidAudiences = configuration.GetSection("AppSettings:Audience").Get<List<string>>(),
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetSection("AppSettings:Issuer").Value,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}