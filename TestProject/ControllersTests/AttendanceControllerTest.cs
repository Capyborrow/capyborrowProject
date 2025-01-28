//using capyborrowProject.Controllers;
//using capyborrowProject.Data;
//using capyborrowProject.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TestProject.Utilities.Factories;

//namespace TestProject.ControllersTests
//{
//    [TestFixture]
//    internal class AttendanceControllerTest
//    {
//        private ApplicationDbContext Context { get; set; }

//        [SetUp]
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase("TestDatabase")
//                .Options;

//            Context = new ApplicationDbContext(options);
//            SeedDatabase();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            Context.Database.EnsureDeleted();
//            Context.Dispose();
//        }

//        [Test]
//        public async Task GetAllAttendances_ShouldReturnOk_WhenDataExists()
//        {
//            var controller = new AttendanceController(Context);
//            var result = await controller.GetAllAttendances();

//            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
//            var okResult = result.Result as OkObjectResult;
//            Assert.That(okResult, Is.Not.Null);
//            Assert.That(okResult.StatusCode, Is.EqualTo(200));
//            var attendances = (IEnumerable<Attendance>)okResult.Value!;
//            Assert.That(attendances, Is.Not.Null);
//        }

//        [Test]
//        public async Task GetAttendance_ShouldReturnOk_WhenIndexInRange()
//        {
//            var controller = new AttendanceController(Context);
//            var result = await controller.GetAttendance(1);

//            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
//            var okResult = result.Result as OkObjectResult;
//            Assert.That(okResult, Is.Not.Null);
//            Assert.That(okResult.StatusCode, Is.EqualTo(200));
//            var attendance = (Attendance)okResult.Value!;
//            Assert.That(attendance.Id, Is.EqualTo(1));
//        }

//        [Test]
//        public async Task GetAttendance_ShouldReturnNotFound_WhenIndexOutOfRange()
//        {
//            var controller = new AttendanceController(Context);
//            var result = await controller.GetAttendance(-1);

//            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
//        }

//        [Test]
//        public async Task PostAttendance_ShouldReturnCreated_WhenDataIsValid()
//        {
//            var controller = new AttendanceController(Context);
//            var attendance = AttendanceFactory.CreateAttendance();

//            var result = await controller.PostAttendance(attendance);

//            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
//            var createdResult = result.Result as CreatedAtActionResult;
//            Assert.That(createdResult, Is.Not.Null);
//            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
//            var createdAttendance = (Attendance)createdResult.Value!;
//            Assert.That(createdAttendance.Id, Is.EqualTo(3));
//        }

//        [Test]
//        public async Task PutAttendance_ShouldReturnBadRequest_WhenDataIsValid()
//        {
//            var controller = new AttendanceController(Context);
//            var updatedAttendance = AttendanceFactory.CreateAttendance();
//            updatedAttendance.Status = Attendance.AttandanceStatus.Absent;

//            var result = await controller.PutAttendance(1, updatedAttendance);

//            Assert.That(result, Is.InstanceOf<BadRequestResult>());
//        }

//        [Test]
//        public async Task PutAttendance_ShouldReturnBadRequest_WhenIdMismatch()
//        {
//            var controller = new AttendanceController(Context);
//            var updatedAttendance = AttendanceFactory.CreateAttendance();

//            var result = await controller.PutAttendance(1, updatedAttendance);

//            Assert.That(result, Is.InstanceOf<BadRequestResult>());
//        }

//        [Test]
//        public async Task DeleteAttendance_ShouldReturnNoContent_WhenIndexInRange()
//        {
//            var controller = new AttendanceController(Context);
//            var result = await controller.DeleteAttendance(1);

//            Assert.That(result, Is.InstanceOf<NoContentResult>());
//        }

//        [Test]
//        public async Task DeleteAttendance_ShouldReturnNotFound_WhenIndexOutOfRange()
//        {
//            var controller = new AttendanceController(Context);
//            var result = await controller.DeleteAttendance(-1);

//            Assert.That(result, Is.InstanceOf<NotFoundResult>());
//        }

//        private void SeedDatabase()
//        {
//            Context.Attendances.AddRange(
//                AttendanceFactory.CreateAttendance(),
//                AttendanceFactory.CreateAttendance()
//            );

//            Context.SaveChanges();
//        }
//    }

//    internal static class AttendanceFactory
//    {
//        public static Attendance CreateAttendance(
//            Attendance.AttandanceStatus status = Attendance.AttandanceStatus.Present)
//        {
//            return new Attendance
//            {
//                Status = status,
//                Student = CreateStudent(),
//                Lesson = CreateLesson()
//            };
//        }

//        public static Attendance CreateDefaultAttendance()
//        {
//            return CreateAttendance(
//                status: Attendance.AttandanceStatus.Present
//            );
//        }

//        private static Student CreateStudent()
//        {
//            return StudentFactory.CreateEmptyStudent();
//        }

//        private static Lesson CreateLesson()
//        {
//            return LessonFactory.CreateEmptyLesson();
//        }
//    }

//}
