using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services.Interfaces;
using System.Data;
using System.Linq;

namespace DBMigration.Services
{
    public class IceTablesRefreshedService : IIceTablesRefreshedService
    {
        IIceTablesRefreshedRepository iceTablesRefreshedRepository;
        DataTable table;
        IConfiguration configuration;


        public IceTablesRefreshedService(IIceTablesRefreshedRepository iceTablesRefreshedRepository, IConfiguration configuration)
        {
            this.iceTablesRefreshedRepository = iceTablesRefreshedRepository;
            this.configuration = configuration;
            this.table = new DataTable();
        }

        public DataTable TablesRefreshed()
        {
            CreateDataTable("IceTablesRefreshed", new List<string>() { "Table Name", "Refreshed" });
            List<TableRefreshTime> tablesRefreshTime = iceTablesRefreshedRepository.GetTablesLastRefresh();
            int expectedRefreshTime = configuration.GetValue<int>("RefreshTime");

            foreach (TableRefreshTime tableRefreshTime in tablesRefreshTime)
            {
                bool refreshed = tableRefreshTime.LastRefreshTime > DateTime.Now.AddMinutes(-expectedRefreshTime);
                table.Rows.Add(tableRefreshTime.TblName, refreshed);
            }
            return table;
        }

        private void CreateDataTable(string featureName, List<string> columns)
        {
            table = new DataTable(featureName);
            foreach (string column in columns) { table.Columns.Add(column); }
        }
    }
}
