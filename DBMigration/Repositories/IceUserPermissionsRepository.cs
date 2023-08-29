using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace DBMigration.Repositories
{
    public class IceUserPermissionsRepository : IIceUserPermissionsRepository
    {
        private readonly IConfiguration configuration;

        public IceUserPermissionsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

       
        public List<IceUserPermissions> GetExpectedPermissions()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                string sql = $@"SELECT Lower(Table_Name) AS Table_Name, Lower(Column_Name) AS Column_Name
                                From IceUserPermissions";
                return connection.Query<IceUserPermissions>(sql).ToList();
            }
        }

        public List<IceUserPermissions> GetActualPermissions(List<string> tableNames)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                var parameters = new { tableNames };

                string sql = $@"SELECT Lower(Table_Name) AS Table_Name, Lower(Column_Name) AS Column_Name
                                FROM INFORMATION_SCHEMA.COLUMNS
                                WHERE Table_Name in @tableNames";
                return connection.Query<IceUserPermissions>(sql, parameters).ToList();
            }
        }




    }
}
