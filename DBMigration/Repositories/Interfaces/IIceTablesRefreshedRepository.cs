using DBMigration.Models;

namespace DBMigration.Repositories.Interfaces
{
    public interface IIceTablesRefreshedRepository
    {
        List<TableRefreshTime> GetTablesLastRefresh();
    }
}