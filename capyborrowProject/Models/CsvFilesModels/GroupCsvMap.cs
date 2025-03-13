namespace capyborrowProject.Models.CsvFilesModels;

using CsvHelper.Configuration;

public class GroupCsvMap : ClassMap<GroupCsvDto>
{
    public GroupCsvMap()
    {
        Map(m => m.GroupName).Name("GroupName");
        Map(m => m.StudentEmail).Name("StudentEmail");
    }
}
