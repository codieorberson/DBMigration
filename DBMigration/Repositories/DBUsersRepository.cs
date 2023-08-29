using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace DBMigration.Repositories
{
    public class DBUsersRepository : IDBUsersRepository
    {
        private readonly IConfiguration configuration;

        public DBUsersRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool DBUserExists(DBUser dbUser)
        {
            string connectionString = $"Server={dbUser.Server};user id={dbUser.User};password={dbUser.Password};Connection Timeout=120;MultiSubnetFailover=True;";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            

        }

        public bool DBUserPermissionExists(DBUser dbUser, string database)
        {
            string connectionString = $"Server={dbUser.Server};user id={dbUser.User};password={dbUser.Password};Database={database};Connection Timeout=120;MultiSubnetFailover=True;";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
        public List<string> GetAllDBUserPermissions(string user)
        {
            string env = configuration.GetValue<string>("ComparisonEnv");

            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { user, env };
                string sql = $@"SELECT [Database]
                                From DBUsers
                                WHERE [User]=@user
                                AND [Environment]=@env";
                return connection.Query<string>(sql, parameters).ToList();
            }
        }

        public List<DBUser> GetAllDBUsers()
        {
            string env = configuration.GetValue<string>("ComparisonEnv");
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { env };

                string sql = $@"SELECT DISTINCT [Server], [User], [Password]
                                From DBUsers
                                WHERE Environment=@env
                                AND [Password] != ''";
                          


                return connection.Query<DBUser>(sql, parameters).ToList();
            }


        }
    }
}
