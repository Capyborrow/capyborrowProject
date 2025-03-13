using capyborrowProject.Models.CsvFilesModels;
using CsvHelper.Configuration;
using System.Globalization;

namespace capyborrowProject.Models.CsvFilesModels
{
    public class AssignmentsCsvMap : ClassMap<AssignmentsCsvDto>
    {
        public AssignmentsCsvMap() 
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }
}
