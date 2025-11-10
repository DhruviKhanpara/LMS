using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMS.Presentation.Middlewares
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtCookieMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
            {
                var handler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value!)),
                    ValidateAudience = true,
                    ValidAudiences = _configuration.GetSection("AppSettings:Audience").Get<List<string>>(),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration.GetSection("AppSettings:Issuer").Value,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                try
                {
                    var principal = handler.ValidateToken(token, validationParameters, out _);
                    context.User = principal;
                }
                catch (Exception)
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }

            await _next(context);
        }
    }
}
