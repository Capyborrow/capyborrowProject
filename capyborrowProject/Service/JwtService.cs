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
            return GenerateJwtToken(payload, _jwtSettings.AccessTokenSecret, TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpiryInMinutes));
        }

        public string GenerateRefreshToken(object payload)
        {
            return GenerateJwtToken(payload, _jwtSettings.RefreshTokenSecret, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryInDays));
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










        //public string GenerateToken(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Email, user.Email),
        //    new Claim(ClaimTypes.Role, user.Role.ToString()),
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _jwtSettings.Issuer,
        //        audience: _jwtSettings.Audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryInMinutes),
        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //public string GenerateJwtToken(object payload, string secret)
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(payload.GetType().GetProperties().Select(p => new Claim(p.Name, p.GetValue(payload)?.ToString()))),
        //        //Expires = DateTime.UtcNow.Add(expiresIn),
        //        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryInMinutes),

        //        SigningCredentials = credentials
        //    };

        //    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    return tokenHandler.WriteToken(token);
        //}


        //public string GenerateRefreshToken()
        //{
        //    return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));  //??
        //}
    }

}
