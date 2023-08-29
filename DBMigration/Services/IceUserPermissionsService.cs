using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services.Interfaces;
using System.Data;
using System.Linq;

namespace DBMigration.Services
{
    public class IceUserPermissionsService: IIceUserPermissionsService
    {
        IIceUserPermissionsRepository iceUserPermissionsRepository;
        DataTable table;


        public IceUserPermissionsService(IIceUserPermissionsRepository iceUserPermissionsRepository)
        {
            this.iceUserPermissionsRepository = iceUserPermissionsRepository;
            this.table = new DataTable();
        }

        public DataTable IceUserPermissionsExist()
        {
            CreateDataTable("MissingIceUserPermissions", new List<string>() { "Table", "Column"});

            List<IceUserPermissions> expectedPermissions = iceUserPermissionsRepository.GetExpectedPermissions();

            List<IceUserPermissions> actualPermissions = iceUserPermissionsRepository.GetActualPermissions(expectedPermissions.Select(x=>x.Table_Name).Distinct().ToList());

            List<IceUserPermissions> missingPermissions = new List<IceUserPermissions>();

            foreach(IceUserPermissions expectedPermission in expectedPermissions)
            {
                List<IceUserPermissions> permissionExists = actualPermissions.Where(x => x.Table_Name == expectedPermission.Table_Name).Where(x => x.Column_Name == expectedPermission.Column_Name).ToList();
                if (permissionExists.Count == 0)
                {
                    missingPermissions.Add(expectedPermission);
                }
            }

            foreach (IceUserPermissions missingPermission in missingPermissions)
            {
                table.Rows.Add(missingPermission.Table_Name, missingPermission.Column_Name);
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
