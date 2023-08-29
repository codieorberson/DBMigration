using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using System.Data.SqlClient;
using Dapper;
using System.Security.AccessControl;

namespace DBMigration.Repositories
{
    public class DBOSRepository : IDBOSRepository
    {
        IConfiguration configuration;

        public DBOSRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<string> GetExpectedLinkedServers()
        {
            string env = configuration.GetValue<string>("SSO2Env");
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { env };

                string sql = $@"SELECT Name
                                FROM LinkedServers
                                WHERE ENV = @env";
                return connection.Query<string>(sql, parameters).ToList();
            }
        }

        public string TestLinkedServer(string linkedServerName)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString($"MetisConnection")))
                {

                    string sql = $@"EXEC sp_testlinkedserver [{linkedServerName}]";
                    connection.Execute(sql);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public List<DBOS> GetExpectedDBOS()
        {
            string env = configuration.GetValue<string>("ComparisonEnv");
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { env };

                string sql = $@"SELECT Lower([Name]) AS [Name], [Object_Type], [Database]
                                FROM DBOS
                                WHERE ENV = @env";
                return connection.Query<DBOS>(sql, parameters).ToList();
            }
        }
        public List<Indexes> GetExpectedIndexes()
        {
            string env = configuration.GetValue<string>("ComparisonEnv");
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { env };
                string sql = $@"SELECT [Database], Lower([Table_Name]) AS [Table_Name], Lower([Column_Name]) AS Column_Name
                                FROM Indexes
                                WHERE ENV = @env";
                return connection.Query<Indexes>(sql, parameters).ToList();
            }
        }

        public List<Schemas> GetExpectedSchemas()
        {
            string env = configuration.GetValue<string>("ComparisonEnv");
            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                var parameters = new { env };
                string sql = $@"SELECT Lower([Name]) AS Name, [Database]
                                FROM Schemas
                                WHERE ENV = @env";
                return connection.Query<Schemas>(sql, parameters).ToList();
            }
        }

        public List<string> GetTables(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Lower([Name]) As Name
                                FROM sys.Tables
                                ORDER BY Name";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetViews(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Lower([Name]) As Name
                                FROM sys.views
                                ORDER BY Name";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetFunctions(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Lower([Name]) As Name
                                FROM sys.objects
                                WHERE type_desc like '%function%'
                                ORDER BY Name";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetSps(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Lower([ROUTINE_NAME]) As Name
                                FROM INFORMATION_SCHEMA.ROUTINES
                                WHERE Routine_Type = 'Procedure'
                                ORDER BY Name";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<Indexes> GetActualIndexes(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Distinct Lower(t.Name) as Table_Name, Lower(col.Name) as Column_Name, 'Metis' AS [Database]
                                FROM 
                                     sys.indexes ind 
                                INNER JOIN 
                                     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                                INNER JOIN 
                                     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                                INNER JOIN 
                                     sys.tables t ON ind.object_id = t.object_id 
                                WHERE 
                                     ind.is_primary_key = 0 
                                     AND ind.is_unique = 0 
                                     AND ind.is_unique_constraint = 0 
                                     AND t.is_ms_shipped = 0";
                return connection.Query<Indexes>(sql).ToList();
            }
        }

        public List<string> GetSchemas(string database)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
            {
                string sql = $@"SELECT Lower(s.name) AS schema_name
                                FROM sys.schemas s
                                INNER JOIN sys.sysusers u ON u.uid = s.principal_id
                                ORDER BY s.name;";
                return connection.Query<string>(sql).ToList();
            }
        }

        public string GetObjectDefinition(string database, string objectName)
        {
            string returnedScript = string.Empty;
            try
            {

                using (var connection = new SqlConnection(configuration.GetConnectionString($"{database}Connection")))
                {

                    string sql = $@"SELECT OBJECT_DEFINITION(ID)
                                FROM sysobjects 
                                WHERE type='P'
                                AND OBJECT_NAME(id) LIKE '%{objectName}%'";

                    returnedScript= connection.Query<string>(sql).FirstOrDefault();
                }
                return returnedScript;
            }
            catch (Exception ex)
            {
                return returnedScript;
            }
            
        }

    }
}
