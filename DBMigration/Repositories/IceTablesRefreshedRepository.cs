using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace DBMigration.Repositories
{
    public class IceTablesRefreshedRepository : IIceTablesRefreshedRepository
    {
        private readonly IConfiguration configuration;

        public IceTablesRefreshedRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

       
        public List<TableRefreshTime> GetTablesLastRefresh()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {

                string sql = $@"SELECT *
                                FROM TableRefreshTime";
                return connection.Query<TableRefreshTime>(sql).ToList();
            }
        }




    }
}
