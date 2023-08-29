using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services.Interfaces;
using System.Data;
using System.Linq;

namespace DBMigration.Services
{
    public class MetisDBOSService : IMetisDBOSService
    {
        IMetisRepository metisRepository;
        IDBOSRepository dboRepository;
        DataTable table;

        public MetisDBOSService(IMetisRepository metisRepository, IDBOSRepository dboRepository)
        {
            this.metisRepository = metisRepository;
            this.dboRepository = dboRepository;
            this.table = new DataTable();
        }

        public DataTable NewMetisDBOSExist()
        {
            CreateDataTable("MetisMissingDBOSFromSalesforce", new List<string>() { "Object Name", "Object Type" });

            List<MetisDBOS> expectedMetisDBOS = metisRepository.GetAllNewMetisDBOS();

            List<string> expectedNewMetisViews = expectedMetisDBOS.Where(x => x.Object_Type == "View").Select(x=>x.Object_Name.ToLower()).ToList();
            List<string> expectedNewMetisTables = expectedMetisDBOS.Where(x => x.Object_Type == "Table").Select(x => x.Object_Name.ToLower()).ToList();
            List<string> expectedNewMetisSps = expectedMetisDBOS.Where(x => x.Object_Type == "Stored Procedure").Select(x => x.Object_Name.ToLower()).ToList();
            List<string> expectedNewMetisFunctions = expectedMetisDBOS.Where(x => x.Object_Type == "Function").Select(x => x.Object_Name.ToLower()).ToList();

            List<string> actualMetisViews = metisRepository.GetAllViews();
            List<string> actualMetisTables = metisRepository.GetAllTables();
            List<string> actualMetisSps = metisRepository.GetAllStoredProcedures();
            List<string> actualMetisFunctions = metisRepository.GetAllFunctions();

            DocumentMissingDBOS(expectedNewMetisViews, actualMetisViews, "View");
            DocumentMissingDBOS(expectedNewMetisTables, actualMetisTables, "Table");
            DocumentMissingDBOS(expectedNewMetisSps, actualMetisSps, "SP");
            DocumentMissingDBOS(expectedNewMetisFunctions, actualMetisFunctions, "Function");

            return table;
        }

        public DataTable MetisDBOSUpdated()
        {
            CreateDataTable("DBOSStillRefSalesforce", new List<string>() { "Database", "Object Name", "Object Type", "Status" });

            List<MetisDBOS> expectedUpdatedDBOS = metisRepository.GetExpectedUpdatedDBOS();
            List<string> expectedUpdatedViews = expectedUpdatedDBOS.Where(x => x.Object_Type == "View").Select(x => x.Object_Name).ToList();
            List<string> expectedUpdatedSps = expectedUpdatedDBOS.Where(x => x.Object_Type == "SP").Select(x => x.Object_Name).ToList();
            List<string> expectedUpdatedFunctions = expectedUpdatedDBOS.Where(x => x.Object_Type == "Function").Select(x => x.Object_Name).ToList();

            List<MetisDBOS> actualUpdatedViews = metisRepository.GetViewsObjectDefinition(expectedUpdatedViews);
            List<MetisDBOS> actualUpdatedSps = metisRepository.GetSpsFunctionsObjectDefinition(expectedUpdatedSps);
            List<MetisDBOS> actualUpdatedFunctions = metisRepository.GetSpsFunctionsObjectDefinition(expectedUpdatedFunctions);

            DocumentMissingBrokenObjects(expectedUpdatedViews, actualUpdatedViews, "View");
            DocumentMissingBrokenObjects(expectedUpdatedSps, actualUpdatedSps, "SP");
            DocumentMissingBrokenObjects(expectedUpdatedFunctions, actualUpdatedFunctions, "Function");

            //Perform one check to see if User_Sync sp was updated as well
            string UserSyncObject = dboRepository.GetObjectDefinition("User_Sync", "usp_BTSync_GetDefaultGroups");
            if (UserSyncObject.ToLower().Contains("salesforce"))
            {
                DocumentBrokenUser_SyncObject();
            }

            return table;
        }

        public DataTable BrokenMetisViews()
        {
            CreateDataTable("BrokenMetisViews", new List<string>() { "View", "Error" });
            IEnumerable<string> metisViews = GetViewNames();
            foreach(string view in metisViews)
            {
                string errorMsg = metisRepository.IsViewBroken(view);
                if (!String.IsNullOrEmpty(errorMsg))
                {
                    table.Rows.Add(view, errorMsg);
                }
                System.Threading.Thread.Sleep(15);
            }

            return table;
        }

        public DataTable PicklistsTableHyrdrated()
        {
            CreateDataTable("PickListTableHyrdrated", new List<string>() { "Hyrdrated" });
            bool hyrdrated = metisRepository.PicklistsHyrdrated();
            table.Rows.Add(hyrdrated);
            return table;
        }

        public DataTable SnapLogicRelatedDBOSUpdated()
        {
            CreateDataTable("SnapLogicRelatedDBOSUpdated", new List<string>() { "Name", "Updated" });
            string trigger = "User_Product_Roles_Alliance_AdminConsoleProfileHistoryOn";
            List<string> snapLogicDbos = new List<string>() { $"{trigger}Delete", $"{trigger}Insert", $"{trigger}Update" };
            foreach (string snapLogicDbo in snapLogicDbos)
            {
                string objectDefinition = metisRepository.GetTriggerObjectDefinition(snapLogicDbo).ToLower();
                bool updated = !objectDefinition.Contains("sf_iamcontact") && !objectDefinition.Contains("sf_iamaccount");
                table.Rows.Add(snapLogicDbo, updated);
            }
            return table;
        }

        private void DocumentBrokenUser_SyncObject()
        {
            table.Rows.Add("User_Sync", "usp_BTSync_GetDefaultGroups", "SP", "Broken");
        }

        private void DocumentMissingDBOS(List<string> expectedUpdatedObjects, List<string> actualUpdatedObjects, string objectType)
        {
            foreach (string expectedUpdatedObject in expectedUpdatedObjects)
            {
                if (!actualUpdatedObjects.Contains(expectedUpdatedObject))
                {
                    table.Rows.Add(objectType, expectedUpdatedObject);
                }
            }
        }

        private void DocumentMissingBrokenObjects(List<string> expectedUpdatedObjects, List<MetisDBOS> actualUpdatedObjects, string objectType)
        {
            
            foreach (string expectedUpdateObject in expectedUpdatedObjects)
            {
                if (actualUpdatedObjects.Where(x => x.Object_Name == expectedUpdateObject).Count() == 0)
                {
                    table.Rows.Add("Metis", expectedUpdateObject, objectType, "Missing");
                    continue;
                }
                string objectDefinition = actualUpdatedObjects.Where(x => x.Object_Name == expectedUpdateObject).Select(x => x.Object_Type).First().ToLower();
                if (objectDefinition.Contains("salesforce"))
                {
                    table.Rows.Add("Metis", expectedUpdateObject, objectType, "Broken");

                }
            }
        }

        private void CreateDataTable(string featureName, List<string> columns)
        {
            table = new DataTable(featureName);
            foreach (string column in columns) { table.Columns.Add(column); }
        }

        private IEnumerable<string> GetViewNames()
        {
            List<string> metisViews = new List<string>();
            List<string> expectedExistingViews = dboRepository.GetExpectedDBOS().Where(x => x.Object_Type == "View").Where(x => x.Database == "Metis").Select(x => x.Name).ToList();
            List<string> expectedNewViews = metisRepository.GetAllNewMetisDBOS().Where(x => x.Object_Type == "View").Select(x => x.Object_Name.ToLower()).ToList();
            return expectedExistingViews.Concat(expectedNewViews);
        }

    }
}
