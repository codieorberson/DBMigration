using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using DBMigration.Services.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Linq;

namespace DBMigration.Services
{
    public class DBOSService : IDBOSService
    {
        IDBOSRepository dbosRepository;
        DataTable table;

        public DBOSService(IDBOSRepository dbosRepository)
        {
            this.dbosRepository = dbosRepository;
            this.table = new DataTable();
        }

        public DataSet DBOSExist()
        {
            List<DBOS> allExpectedDBOS = dbosRepository.GetExpectedDBOS();
            List<Indexes> allExpectedIndexes = dbosRepository.GetExpectedIndexes();
            List<Schemas> allExpectedSchemas = dbosRepository.GetExpectedSchemas();
            DataSet dataSet = new DataSet("DBOSExist");

            List<string> databases = new List<string>() { "Metis", "EWSDataStore", "UHC_SMD", "UHCCore", "UHCNotification", "User_Sync", "SDE2014", "SSOLogging" };
            foreach (string database in databases)
            {
                table = new DataTable(database);
                table.Columns.Add("Object Name");
                table.Columns.Add("Object Type");
                List<DBOS> expectedDBOS = allExpectedDBOS.Where(x => x.Database == database).ToList();
                List<Indexes> expectedIndexes = allExpectedIndexes.Where(x => x.Database == database).ToList();
                List<string> expectedSchemas = allExpectedSchemas.Where(x => x.Database == database).Select(x => x.Name).ToList();
                List<string> expectedTables = expectedDBOS.Where(x => x.Object_Type == "Table").Select(x => x.Name).ToList();
                List<string> expectedViews = expectedDBOS.Where(x => x.Object_Type == "View").Select(x => x.Name).ToList();
                List<string> expectedFunctions = expectedDBOS.Where(x => x.Object_Type == "Function").Select(x => x.Name).ToList();
                List<string> expectedSps = expectedDBOS.Where(x => x.Object_Type == "SP").Select(x => x.Name).ToList();

                List<Indexes> actualIndexes = dbosRepository.GetActualIndexes(database);
                List<string> actualSchemas = dbosRepository.GetSchemas(database);
                List<string> actualTables = dbosRepository.GetTables(database);
                List<string> actualViews = dbosRepository.GetViews(database);
                List<string> actualFunctions = dbosRepository.GetFunctions(database);
                List<string> actualSps = dbosRepository.GetSps(database);

                DocumentMissingObjects(expectedSchemas, actualSchemas, "Schema", database);
                DocumentMissingIndexes(expectedIndexes, actualIndexes, database);
                DocumentMissingObjects(expectedTables, actualTables, "Table", database);
                DocumentMissingObjects(expectedViews, actualViews, "View", database);
                DocumentMissingObjects(expectedFunctions, actualFunctions, "Function", database);
                DocumentMissingObjects(expectedSps, actualSps, "SP", database);
                dataSet.Tables.Add(table);

            }

            return dataSet;
        }

        public DataTable LinkedServersFunctioning()
        {
            CreateDataTable("LinkedServersFunctioning", new List<string>() { "Name", "Valid Connection", "ErrorMessage" });

            List<string> expectedLinkedServers = dbosRepository.GetExpectedLinkedServers();
            foreach(string expectedLinkedServer in expectedLinkedServers)
            {
                string validMessage = dbosRepository.TestLinkedServer(expectedLinkedServer);
                bool valid = validMessage == string.Empty ? true : false;
                table.Rows.Add(expectedLinkedServer, valid, validMessage);
            }
            return table;
        }
        private void DocumentMissingIndexes(List<Indexes> expectedIndexes, List<Indexes> actualIndexes, string database)
        {
            List<string> expectedIndexesTables = expectedIndexes.Select(x => x.Table_Name).Distinct().ToList();
            foreach (string expectedIndexesTable in expectedIndexesTables)
            {
                List<string> expectedIndexesColumns = expectedIndexes.Where(x => x.Table_Name == expectedIndexesTable).Select(x => x.Column_Name).ToList();
                List<string> actualIndexesColumns = actualIndexes.Where(x => x.Table_Name == expectedIndexesTable).Select(x => x.Column_Name).ToList();
                foreach (string expectedIndexesColumn in expectedIndexesColumns)
                {
                    if (!actualIndexesColumns.Contains(expectedIndexesColumn))
                    {
                        table.Rows.Add($"{expectedIndexesTable} {expectedIndexesColumn}", "Index");
                    }
                }
            }
        }

        private void DocumentMissingObjects(List<string> expectedDBOS, List<string> actualDBOS, string objectType, string database)
        {
            foreach (string expectedDBO in expectedDBOS)
            {
                if (!actualDBOS.Contains(expectedDBO))
                {
                    table.Rows.Add(expectedDBO, objectType);
                }
            }
        }

        private void CreateDataTable(string featureName, List<string> columns)
        {
            table = new DataTable(featureName);
            foreach (string column in columns) { table.Columns.Add(column); }
        }

    }
}
