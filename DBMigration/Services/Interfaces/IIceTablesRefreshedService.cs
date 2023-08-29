using System.Data;

namespace DBMigration.Services.Interfaces
{
    public interface IIceTablesRefreshedService
    {
        DataTable TablesRefreshed();
    }
}