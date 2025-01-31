using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.PredefinedTables
{
    public class AssignmentStatus
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public required string Name { get; set; }
    }
}
