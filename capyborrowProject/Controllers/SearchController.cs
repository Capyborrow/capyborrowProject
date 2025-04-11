using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Search([FromQuery] string q = "*", [FromQuery] string? group = null)
        {
            string? filter = group != null ? $"GroupTitle_uk eq '{group}'" : null;
            var results = await _searchService.SearchAsync(q, filter);
            return Ok(results);
        }
    }
}
