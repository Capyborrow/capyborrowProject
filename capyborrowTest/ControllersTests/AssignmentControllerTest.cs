using capyborrowProject.Controllers;
using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace capyborrowTest.ControllersTests
{
    [TestFixture]
    internal class AssignmentControllerTest
    {
        public APIContext Context { get; private set; }
        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<APIContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            Context = new APIContext(options);

            SeedDatabase();
        }
        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }

        [Test]
        async public Task Test()
        {
            // Arrange
            var controller = new AssignmentController(Context);

            // Act
            var result = await controller.GetAllAssignments();           
            
            // Assert
            
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));


            var returnedAssignments = okResult.Value as List<Assignment>;
            Assert.That(returnedAssignments, Is.Not.Null);

            //should check if returned values are equal but with less hardcoding
            //Assert.That(returnedAssignments.Count, Is.EqualTo(2));
            //Assert.That("Assignment 1", Is.EqualTo(returnedAssignments[0].title));
        }

        //should be added to DataSource
        private void SeedDatabase()
        {
            var teacher = new Teacher
            {
                email = "teacher@example.com",
                firstName = "John",
                lastName = "Doe",
                passwordHash = "hashed_password"
            };

            Context.Assignments.AddRange(
                new Assignment { Id = 1, title = "Assignment 1", teacher = teacher },
                new Assignment { Id = 2, title = "Assignment 2", teacher = teacher }
            );

            Context.SaveChanges();
        }
    }    
}
