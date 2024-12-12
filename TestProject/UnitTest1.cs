using capyborrowProject.Controllers;
using Microsoft.AspNetCore.Mvc;

// For testing. Will be replaced or deleted later 

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Get_ReturnsHelloWorld()
        {
            var controller = new APIController();
            var result = controller.Get() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Hello, world!", result.Value);
        }
    }
}