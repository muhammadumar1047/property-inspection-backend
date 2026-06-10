using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PropertyInspection.Application.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(IEnumerable<Claim> claims, bool rememberMe = false)
        {
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var jwtKey = _configuration["Jwt:Key"];

            // Read token expiry from configuration, with fallback defaults
            var defaultExpiry = _configuration.GetValue<int>("Jwt:TokenExpiryMinutes", 120);
            var rememberMeExpiry = _configuration.GetValue<int>("Jwt:TokenExpiryMinutesRememberMe", 10080);
            var expiryMinutes = rememberMe ? rememberMeExpiry : defaultExpiry;

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
