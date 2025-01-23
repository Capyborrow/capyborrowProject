using capyborrowProject.Controllers;
using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestProject.Utilities.Factories;

namespace TestProject.ControllersTests
{
    [TestFixture]
    internal class AssignmentControllerTest
    {
        private APIContext Context { get; set; }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<APIContext>()
                .UseInMemoryDatabase("TestDatabase")
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
        public async Task GetAllAssignments_ShouldReturnOk_WhenDataExists()
        {
            var controller = new AssignmentController(Context);
            var result = await controller.GetAllAssignments();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var assignments = (IEnumerable<Assignment>)okResult.Value!;
            Assert.That(assignments, Is.Not.Null);
        }

        [Test]
        public async Task GetAssignment_ShouldReturnOk_WhenIndexInRange()
        {
            var controller = new AssignmentController(Context);
            var result = await controller.GetAssignment(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var assignment = (Assignment)okResult.Value!;
            Assert.That(assignment.Title, Is.EqualTo("Test Assignment 1"));
        }

        [Test]
        public async Task GetAssignment_ShouldReturnNotFound_WhenIndexOutOfRange()
        {
            var controller = new AssignmentController(Context);
            var result = await controller.GetAssignment(-1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task PostAssignment_ShouldReturnOk_WhenDataIsValid()
        {
            var controller = new AssignmentController(Context);
            var assignment = AssignmentFactory.CreateValidAssignment();

            var result = await controller.PostAssignment(assignment);

            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var okResult = result.Result as CreatedAtActionResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(201));
            var createdAssignment = (Assignment)okResult.Value!;
            Assert.That(createdAssignment.Title, Is.EqualTo("Test Assignment"));
        }

        [Test]
        public async Task DeleteAssignment_ShouldReturnNoContent_WhenIndexInRange()
        {
            var controller = new AssignmentController(Context);
            var result = await controller.DeleteAssignment(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteAssignment_ShouldReturnNotFound_WhenIndexOutOfRange()
        {
            var controller = new AssignmentController(Context);
            var result = await controller.DeleteAssignment(-1);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        private void SeedDatabase()
        {
            Context.Assignments.AddRange(
                AssignmentFactory.CreateAssignment(1, "Test Assignment 1"),
                AssignmentFactory.CreateAssignment(2, "Test Assignment 2")
            );

            Context.SaveChanges();
        }
    }

    internal static class AssignmentFactory
    {
        public static Assignment CreateValidAssignment() => new()
        {
            Title = "Test Assignment",
            Status = Assignment.TaskStatus.ToDo,
            DateAssigned = DateTime.UtcNow,
            Lesson = CreateLesson(),
            Students = []
        };

        public static Assignment CreateAssignment(int id, string title) => new()
        {
            Id = id,
            Title = title,
            Status = Assignment.TaskStatus.ToDo,
            DateAssigned = DateTime.UtcNow,
            Lesson = CreateLesson(),
            Students = []
        };

        private static Lesson CreateLesson() => LessonFactory.CreateEmptyLesson();
    }
}
