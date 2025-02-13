using capyborrowProject.Controllers;
using capyborrowProject.Data;
using capyborrowProject.Models.AuthModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestProject.Utilities.AuthUtils;

namespace TestProject.ControllersTests
{
    [TestFixture]
    public class AuthController_Register_Tests
    {
        private ApplicationDbContext _context;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _authController = new(
                _context,
                UserManagerFactory.Create(),
                RoleManagerFactory.Create(),
                JwtServiceFactory.Create(),
                new FakeEmailService());
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            var request = new RegisterRequest
            {
                FirstName = "",
                MiddleName = "A",
                LastName = "Doe",
                Email = "invalid@example.com",
                Role = "student",
                Password = "Password123!"
            };

            _authController.ModelState.AddModelError("FirstName", "FirstName is required");

            var result = await _authController.Register(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            StringAssert.Contains("Wrong register credentials", badRequest!.Value!.ToString());
        }

        [Test]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            var request = new RegisterRequest
            {
                FirstName = "John",
                MiddleName = "A",
                LastName = "Doe",
                Email = "john@example.com",
                Role = "student",
                Password = "Password123!"
            };

            _authController.ModelState.Clear();

            var firstResult = await _authController.Register(request);
            Assert.That(firstResult, Is.InstanceOf<CreatedResult>());

            var secondResult = await _authController.Register(request);

            Assert.That(secondResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = secondResult as BadRequestObjectResult;
            StringAssert.Contains("A user with this email already exists.", badRequest!.Value!.ToString());
        }

        [Test]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            var request = new RegisterRequest
            {
                FirstName = "John",
                MiddleName = "A",
                LastName = "Doe",
                Email = "john2@example.com",
                Role = "admin",
                Password = "Password123!"
            };

            _authController.ModelState.Clear();

            var result = await _authController.Register(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            StringAssert.Contains("Invalid role", badRequest!.Value!.ToString());
        }

        [Test]
        public async Task Register_ValidStudentRegistration_ReturnsCreatedResult()
        {
            var request = new RegisterRequest
            {
                FirstName = "John",
                MiddleName = "A",
                LastName = "Doe",
                Email = "john3@example.com",
                Role = "student",
                Password = "Password123!"
            };

            _authController.ModelState.Clear();

            var result = await _authController.Register(request);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
            var createdResult = result as CreatedResult;
            Assert.That(createdResult!.Location, Is.EqualTo(request.Email));

            var responseJson = JsonConvert.SerializeObject(createdResult.Value);
            var response = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.That((string)response!.Message, Is.EqualTo("User registered successfully."));
        }

        [Test]
        public async Task Register_ValidTeacherRegistration_ReturnsCreatedResult()
        {
            var request = new RegisterRequest
            {
                FirstName = "Jane",
                MiddleName = "B",
                LastName = "Smith",
                Email = "jane@example.com",
                Role = "teacher",
                Password = "Password123!"
            };

            _authController.ModelState.Clear();

            var result = await _authController.Register(request);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
            var createdResult = result as CreatedResult;
            Assert.That(createdResult!.Location, Is.EqualTo(request.Email));

            var responseJson = JsonConvert.SerializeObject(createdResult.Value);
            var response = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.That((string)response!.Message, Is.EqualTo("User registered successfully."));
        }
    }
}
