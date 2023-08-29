using System.Data;

namespace DBMigration.Services.Interfaces
{
    public interface IIceUserPermissionsService
    {
        DataTable IceUserPermissionsExist();
    }
}