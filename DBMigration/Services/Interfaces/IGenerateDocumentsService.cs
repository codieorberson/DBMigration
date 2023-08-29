using DBMigration.Models;

namespace DBMigration.Services
{
    public interface IGenerateDocumentsService
    {
        void DBUsersExist(DBUser dbUser, bool exists, string documentPath);
        void DBUsersPermissionsExist(string dbUser, string database, bool exists, string documentPath);
        void IceUserPermissions(IceUserPermissions missingPermission, string documentPath);
        void IceTablesRefreshed(TableRefreshTime tableRefreshTime, bool refreshed, string documentPath);
        void AddBrokenMetisObject(string metisObject, string metisObjectType, string documentPath);
        void AddMissingMetisObject(string metisObject, string metisObjectType, string documentPath);
        void AddMissingDBO(string dbObject, string dbObjectType, string documentPath, string database);
        void AddBrokenMetisView(string view, string error, string documentPath);
        void LinkedServerFunctioning(string linkedServerName, bool valid, string validMessage, string documentPath);
    }
}