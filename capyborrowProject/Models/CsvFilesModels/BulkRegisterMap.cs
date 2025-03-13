using CsvHelper.Configuration;
using System.Globalization;

namespace capyborrowProject.Models.CsvFilesModels
{
    public class BulkRegisterMap : ClassMap<BulkRegister>
    {
        public BulkRegisterMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }
}
