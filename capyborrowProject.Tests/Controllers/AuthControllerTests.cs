using AutoFixture;
using AutoFixture.AutoMoq;
using capyborrowProject.Controllers;
using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Models.AuthModels;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace capyborrowProject.Tests.Controllers
{
    [TestFixture]
    class AuthControllerTests
    {
        [Test]
        public async Task AuthController_Register_UserExists()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            string email = "testEmail@ko.ko";
            var userManagerMock = fixture.Create<Mock<UserManager<ApplicationUser>>>();

            var existingUser = fixture.Build<ApplicationUser>()
                .With(u => u.Email, email)
                .Create();
            userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(existingUser);

            var request = fixture.Build<RegisterRequest>()
                .With(r => r.Email, email)
                .Create();

            var sut = GetAuthController(userManager: userManagerMock.Object);

            var result = await sut.Register(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AuthController_Login_UserNotFound()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            string email = "testEmail@ko.ko";
            var userManagerMock = fixture.Create<Mock<UserManager<ApplicationUser>>>();
            userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser?)null);

            var request = fixture.Build<LoginRequest>()
                .With(r => r.Email, email)
                .Create();

            var sut = GetAuthController(userManager: userManagerMock.Object);

            var result = await sut.Login(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AuthController_ForgotPassword_UserNotFound()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            string email = "testEmail@ko.ko";
            var userManagerMock = fixture.Create<Mock<UserManager<ApplicationUser>>>();
            userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser?)null);
            var request = fixture.Build<ForgotPasswordRequest>()
                .With(r => r.Email, email)
                .Create();

            var sut = GetAuthController(userManager: userManagerMock.Object);

            var result = await sut.ForgotPassword(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AuthController_ConfirmEmail_InvalidToken()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            string email = "testEmail@ko.ko";
            string token = "invalid_token";
            var userManagerMock = fixture.Create<Mock<UserManager<ApplicationUser>>>();
            var existingUser = fixture.Build<ApplicationUser>()
                .With(u => u.Email, email)
                .Create();
            userManagerMock
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(existingUser);
            userManagerMock
                .Setup(um => um.ConfirmEmailAsync(existingUser, token))
                .ReturnsAsync(IdentityResult.Failed());
            var request = fixture.Build<ConfirmEmailRequest>()
                .With(r => r.Email, email)
                .With(r => r.Token, token)
                .Create();

            var sut = GetAuthController(userManager: userManagerMock.Object);

            var result = await sut.ConfirmEmail(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        private static AuthController GetAuthController(
            ApplicationDbContext? context = null,
            UserManager<ApplicationUser>? userManager = null,
            RoleManager<IdentityRole>? roleManager = null,
            JwtService? jwtService = null,
            EmailService? emailService = null)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            context ??= fixture.Create<Mock<ApplicationDbContext>>().Object;
            userManager ??= fixture.Create<UserManager<ApplicationUser>>();
            roleManager ??= fixture.Create<RoleManager<IdentityRole>>();
            jwtService ??= fixture.Create<Mock<JwtService>>().Object;
            emailService ??= fixture.Create<Mock<EmailService>>().Object;

            return new AuthController(context, userManager, roleManager, jwtService, emailService);
        }
    }
}
