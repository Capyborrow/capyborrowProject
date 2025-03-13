using CsvHelper.Configuration;
using System.Globalization;

namespace capyborrowProject.Models.CsvFilesModels;

public class SubjectCsvMap : ClassMap<SubjectCsvDto>
{
    public SubjectCsvMap()
    {
        Map(m => m.Name).Name("Name");
    }
}
