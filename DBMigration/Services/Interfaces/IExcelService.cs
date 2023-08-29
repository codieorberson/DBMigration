using DBMigration.Models;
using System.Data;

namespace DBMigration.Services
{
    public interface IExcelService
    {
        void SaveDataSetAsExcel(DataSet dataset, string excelFilePath);
    }
}