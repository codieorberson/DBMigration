using System.Data;

namespace DBMigration.Services.Interfaces
{
    public interface IDBUsersService
    {
        DataTable DBUsersExist();
        DataTable DBUsersPermissionsExist();
    }
}