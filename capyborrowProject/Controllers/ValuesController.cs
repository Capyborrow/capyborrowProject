using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using capyborrowProject.Service;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q = "*", 
                                                [FromQuery] string? group = null, 
                                                [FromQuery] string? type = null, 
                                                [FromQuery] string? status = null)
        {
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(group))
                filters.Add($"GroupTitle eq '{group}'");

            if (!string.IsNullOrWhiteSpace(type))
                filters.Add($"LessonTypeName eq '{type}'");

            if (!string.IsNullOrWhiteSpace(status))
                filters.Add($"LessonStatusName eq '{status}'");

            string? filter = filters.Any() ? string.Join(" and ", filters) : null;

            var results = await _searchService.SearchAsync(q, filter);
            return Ok(results);
        }
    }
}
