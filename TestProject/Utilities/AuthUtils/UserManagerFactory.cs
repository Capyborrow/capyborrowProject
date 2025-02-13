using capyborrowProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utilities.AuthUtils
{
    internal static class UserManagerFactory
    {
        public static UserManager<ApplicationUser> Create()
        {
            var storeMock = new Mock<IUserStore<ApplicationUser>>();

            storeMock.As<IUserPasswordStore<ApplicationUser>>()
                .Setup(s => s.SetPasswordHashAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            storeMock.As<IUserPasswordStore<ApplicationUser>>()
                .Setup(s => s.GetPasswordHashAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("password-hash");
            storeMock.As<IUserPasswordStore<ApplicationUser>>()
                .Setup(s => s.HasPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            storeMock.As<IUserRoleStore<ApplicationUser>>()
                .Setup(s => s.GetRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string>());
            storeMock.As<IUserRoleStore<ApplicationUser>>()
                .Setup(s => s.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            storeMock.As<IUserClaimStore<ApplicationUser>>()
                .Setup(s => s.GetClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Claim>());
            storeMock.As<IUserClaimStore<ApplicationUser>>()
                .Setup(s => s.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            storeMock.As<IUserEmailStore<ApplicationUser>>()
                .Setup(s => s.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationUser)null);

            storeMock.Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(IdentityResult.Success);

            var options = new OptionsWrapper<IdentityOptions>(new IdentityOptions());
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var userValidators = new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() };
            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var services = new ServiceCollection().BuildServiceProvider();
            var logger = new Logger<UserManager<ApplicationUser>>(new LoggerFactory());

            var tokenProviderMock = new Mock<IUserTwoFactorTokenProvider<ApplicationUser>>();
            tokenProviderMock.Setup(tp => tp.CanGenerateTwoFactorTokenAsync(It.IsAny<UserManager<ApplicationUser>>(), It.IsAny<ApplicationUser>()))
                             .ReturnsAsync(true);
            tokenProviderMock.Setup(tp => tp.GenerateAsync(It.IsAny<string>(), It.IsAny<UserManager<ApplicationUser>>(), It.IsAny<ApplicationUser>()))
                             .ReturnsAsync("test-token");
            tokenProviderMock.Setup(tp => tp.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserManager<ApplicationUser>>(), It.IsAny<ApplicationUser>()))
                             .ReturnsAsync(true);

            var tokenProviders = new Dictionary<string, IUserTwoFactorTokenProvider<ApplicationUser>>
        {
            { TokenOptions.DefaultProvider, tokenProviderMock.Object }
        };

            var userManager = new UserManager<ApplicationUser>(
                storeMock.Object,
                options,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger
            );

            userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, tokenProviderMock.Object);

            return userManager;
        }
    }
}
