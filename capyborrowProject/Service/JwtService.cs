using capyborrowProject.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace capyborrowProject.Service
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IConfiguration configuration)
        {
            _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        }

        public string GenerateAccessToken(object payload)
        {
            return GenerateJwtToken(payload, _jwtSettings.AccessTokenSecret, TimeSpan.FromSeconds(_jwtSettings.AccessTokenExpiryInSeconds));
        }

        public string GenerateRefreshToken(object payload)
        {
            return GenerateJwtToken(payload, _jwtSettings.RefreshTokenSecret, TimeSpan.FromSeconds(_jwtSettings.RefreshTokenExpiryInSeconds));
        }

        private string GenerateJwtToken(object payload, string secret, TimeSpan expiresIn)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = payload.GetType()
                .GetProperties()
                .Select(p => new Claim(p.Name, p.GetValue(payload)?.ToString() ?? string.Empty))
                .ToList();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiresIn),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            return ValidateJwtToken(token, _jwtSettings.AccessTokenSecret);
        }

        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            return ValidateJwtToken(token, _jwtSettings.RefreshTokenSecret);
        }

        private ClaimsPrincipal? ValidateJwtToken(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }


    }
}
