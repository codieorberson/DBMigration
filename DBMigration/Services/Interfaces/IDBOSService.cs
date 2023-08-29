using System.Data;

namespace DBMigration.Services.Interfaces
{
    public interface IDBOSService
    {
        DataSet DBOSExist();
        DataTable LinkedServersFunctioning();
    }
}