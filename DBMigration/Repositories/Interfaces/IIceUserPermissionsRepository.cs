using DBMigration.Models;

namespace DBMigration.Repositories.Interfaces
{
    public interface IIceUserPermissionsRepository
    {
        List<IceUserPermissions> GetExpectedPermissions();
        List<IceUserPermissions> GetActualPermissions(List<string> tableNames);
    }
}