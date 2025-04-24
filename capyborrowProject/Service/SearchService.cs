using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using capyborrowProject.Models.SearchDTO;


namespace capyborrowProject.Service
{
    public class SearchService
    {
        private readonly SearchClient _searchClient;

        public SearchService(IConfiguration config)
        {
            var endpoint = new Uri(config["AzureSearch:Endpoint"]!);
            var apiKey = new AzureKeyCredential(config["AzureSearch:ApiKey"]!);
            var indexName = config["AzureSearch:IndexName"]!;
            _searchClient = new SearchClient(endpoint, indexName, apiKey);
        }

        public async Task<List<SearchLessonDto>> SearchAsync(string query, string? filter = null)
        {
            var options = new SearchOptions
            {
                IncludeTotalCount = true,
                Filter = filter,
                Size = 20,
                SearchFields =
                {
                    "Room",
                    "SubjectTitle_en",
                    "SubjectTitle_uk",
                    "TeacherFullName_en",
                    "TeacherFullName_uk",
                    "TeacherEmail"
                },
                //Select = {
                //    "LessonId", "LessonDate",
                //    "LessonTypeName", "LessonStatusName",
                //    "Room",
                //    "SubjectTitle_en", "SubjectTitle_uk",
                //    "TeacherFullName_en", "TeacherFullName_uk",
                //    "TeacherEmail"
                //}
            };

            var results = await _searchClient.SearchAsync<SearchLessonDto>(query, options);

            return results.Value.GetResults()
                .Select(r => r.Document)
                .ToList();
        }
    }
}
