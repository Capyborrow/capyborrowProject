using CsvHelper.Configuration;
using System.Globalization;

namespace capyborrowProject.Models.CsvFilesModels;

public class LessonCsvMap : ClassMap<LessonCsvDto>
{
    public LessonCsvMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
    }
}
