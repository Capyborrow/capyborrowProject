using capyborrowProject.Controllers;
using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowTest.ControllersTests
{
    [TestFixture]
    internal class AssignmentControllerTest
    {
        private APIContext Context { get; set; }

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
        async public Task GetAllAssignments_ShouldReturnOk_WhenDataExists()
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

        [Test]
        async public Task GetAssignment_ShouldReturnOk_WhenIndexInRange()
        {
            // Arrange
            var controller = new AssignmentController(Context);

            // Act
            var result = await controller.GetAssignment(1);

            // Assert

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));


            var returnedAssignment = okResult.Value as Assignment;
            Assert.That(returnedAssignment, Is.Not.Null);

            //should check if returned values are equal but with less hardcoding
            //Assert.That(returnedAssignments.Count, Is.EqualTo(2));
            Assert.That(returnedAssignment.Title, Is.EqualTo("Assignment 1"));
        }

        [Test]
        async public Task GetAssignment_ShouldReturnNotFound_WhenIndexOutOfRange()
        {
            // Arrange
            var controller = new AssignmentController(Context);

            // Act
            var result = await controller.GetAssignment(-1);

            // Assert

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());

            var notFoundResult = result.Result as NotFoundResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
        }

        [Test]
        async public Task PostAssignment_ShouldReturnOk_WhenDataIsValid()
        {
            // Arrange
            var controller = new AssignmentController(Context);
            //should be put to datasource
            Assignment assignment = new()
            {
                Title = "Test Assignment",
                Status = Assignment.TaskStatus.ToDo,
                DateAssigned = DateTime.UtcNow,
                Lesson = new()
                {
                    Attendances = new List<Attendance>(),
                    Date = DateTime.UtcNow,
                    Group = new()
                    {
                        Name = "",
                        Students = [],
                    },
                    Importance = Lesson.LessonImportance.Test,
                    Location = "",
                    Subject = new()
                    {
                        Teacher = new()
                        {
                            Email = "",
                            FirstName = "",
                            LastName = "",
                            Notifications = [],
                            PasswordHash = "",
                            Role = 1,
                            Subjects = [],
                        },
                        Title = "",
                    },
                    Type = Lesson.LessonType.Lecture,
                },
                Students = [],
            };

            // Act
            var result = await controller.PostAssignment(assignment);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result.Result as OkObjectResult;
            Assert.That(okObjectResult?.StatusCode, Is.EqualTo(200));

        }

        [Test]
        async public Task DeleteAssignment_ShouldReturnNoContent_IndexInRange()
        {
            // Arrange
            var controller = new AssignmentController(Context);

            // Act
            var result = await controller.DeleteAssignment(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult?.StatusCode, Is.EqualTo(204));

        }

        [Test]
        async public Task DeleteAssignment_ShouldReturnNotFound_IndexOutOfRange()
        {
            // Arrange
            var controller = new AssignmentController(Context);

            // Act
            var result = await controller.DeleteAssignment(-1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());

            var notFoundResult = result as NotFoundResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));

        }


        //should be added to DataSource
        private void SeedDatabase()
        {


            Context.Assignments.AddRange(
                new Assignment
                {
                    Title = "Test Assignment 1",
                    Status = Assignment.TaskStatus.ToDo,
                    DateAssigned = DateTime.UtcNow,
                    Lesson = new()
                    {
                        Attendances = new List<Attendance>(),
                        Date = DateTime.UtcNow,
                        Group = new()
                        {
                            Name = "",
                            Students = [],
                        },
                        Importance = Lesson.LessonImportance.Test,
                        Location = "",
                        Subject = new()
                        {
                            Teacher = new()
                            {
                                Email = "",
                                FirstName = "",
                                LastName = "",
                                Notifications = [],
                                PasswordHash = "",
                                Role = 1,
                                Subjects = [],
                            },
                            Title = "",
                        },
                        Type = Lesson.LessonType.Lecture,
                    },
                    Students = [],
                },
                new Assignment
                {
                    Title = "Test Assignment 2",
                    Status = Assignment.TaskStatus.ToDo,
                    DateAssigned = DateTime.UtcNow,
                    Lesson = new()
                    {
                        Attendances = new List<Attendance>(),
                        Date = DateTime.UtcNow,
                        Group = new()
                        {
                            Name = "",
                            Students = [],
                        },
                        Importance = Lesson.LessonImportance.Test,
                        Location = "",
                        Subject = new()
                        {
                            Teacher = new()
                            {
                                Email = "",
                                FirstName = "",
                                LastName = "",
                                Notifications = [],
                                PasswordHash = "",
                                Role = 1,
                                Subjects = [],
                            },
                            Title = "",
                        },
                        Type = Lesson.LessonType.Lecture,
                    },
                    Students = [],
                }
            );

            Context.SaveChanges();
        }
    }
}
