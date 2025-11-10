using LMS.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LMS.Common.Security
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpContext? _httpContext;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #region Create Token
        public string CreateToken(string email, string role, long id, string name, string? profilePhoto)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim("profilePhoto", profilePhoto ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _httpContext!.GetBaseURL(),
                audience: _httpContext!.GetOriginBaseURL(),
                claims: claims,
                signingCredentials: cred,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration.GetSection("AppSettings:jwtTokenExpirationMinutes").Value!))
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        #endregion
    }
}
