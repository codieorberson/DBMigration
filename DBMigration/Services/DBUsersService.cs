using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services.Interfaces;
using System.Data;

namespace DBMigration.Services
{
    public class DBUsersService: IDBUsersService
    {
        IDBUsersRepository dbUsersRepository;
        DataTable table;


        public DBUsersService(IDBUsersRepository dbUsersRepository)
        {
            this.dbUsersRepository = dbUsersRepository;
            this.table = new DataTable();
        }

        public DataTable DBUsersExist()
        {
            CreateDataTable("SSODBUsersExist", new List<string>() { "DBUser", "Exists" });

            List<DBUser> dbUsers = dbUsersRepository.GetAllDBUsers();

            foreach(DBUser dbUser in dbUsers)
            {
                bool exists = dbUsersRepository.DBUserExists(dbUser);
                table.Rows.Add(dbUser.User, exists);
            }

            return table;

        }

        public DataTable DBUsersPermissionsExist()
        {

            CreateDataTable("SSODBUsersPermissionsExist", new List<string>() { "DBUser", "Database", "Exists" });

            List<DBUser> dbUsers = dbUsersRepository.GetAllDBUsers();

            foreach( DBUser dbUser in dbUsers)
            {
                dbUser.Databases = dbUsersRepository.GetAllDBUserPermissions(dbUser.User);
            }

            foreach (DBUser dbUser in dbUsers)
            {
                foreach(string db in dbUser.Databases)
                {
                    bool exists = dbUsersRepository.DBUserPermissionExists(dbUser, db);
                    table.Rows.Add(dbUser.User, db, exists);

                }
            }
            return table;
        }
        private void CreateDataTable(string featureName, List<string> columns)
        {
            table = new DataTable(featureName);
            foreach(string column in columns) { table.Columns.Add(column); }
        }
    }
}
