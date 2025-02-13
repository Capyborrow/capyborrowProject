using capyborrowProject.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utilities.AuthUtils
{
    internal static class JwtServiceFactory
    {
        public static JwtService Create()
        {
            var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:AccessTokenSecret", "your-access-token-secret-key" },
            { "Jwt:AccessTokenExpiryInSeconds", "3600" },
            { "Jwt:RefreshTokenSecret", "your-refresh-token-secret-key" },
            { "Jwt:RefreshTokenExpiryInSeconds", "86400" },
            { "Jwt:Issuer", "your-issuer" },
            { "Jwt:Audience", "your-audience" }
        };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new JwtService(configuration);
        }
    }
}
