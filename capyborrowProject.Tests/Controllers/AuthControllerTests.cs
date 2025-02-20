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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cappyborrowProject.Tests.Controllers
{
    [TestFixture]
    class AuthControllerTests
    {
        [Test]
        public async Task AuthController_Register_UserExists()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            // Arrange
            string email = "testEmail@ko.ko";
    
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

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

            // Act
            var result = await sut.Register(request);

            // Assert
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
