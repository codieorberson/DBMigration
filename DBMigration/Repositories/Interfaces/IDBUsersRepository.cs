using DBMigration.Models;

namespace DBMigration.Repositories.Interfaces
{
    public interface IDBUsersRepository
    {
        bool DBUserExists(DBUser dbUser);
        bool DBUserPermissionExists(DBUser dbUser, string database);
        List<DBUser> GetAllDBUsers();
        List<string> GetAllDBUserPermissions(string user);
    }
}