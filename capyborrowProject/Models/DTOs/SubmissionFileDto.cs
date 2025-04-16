namespace capyborrowProject.Models.DTOs
{
    public class SubmissionFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
    }
}
