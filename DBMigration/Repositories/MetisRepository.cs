using DBMigration.Models;
using DBMigration.Repositories.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace DBMigration.Repositories
{
    public class MetisRepository : IMetisRepository
    {
        private readonly IConfiguration configuration;

        public MetisRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        
        public List<MetisDBOS> GetAllNewMetisDBOS()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                string sql = $@"SELECT [Object_Name], [Object_Type]
                                FROM NewMetisDBOS";
                return connection.Query<MetisDBOS>(sql).ToList();
            }
        }

        public List<MetisDBOS> GetExpectedUpdatedDBOS()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("DBMigrationConnection")))
            {
                string sql = $@"SELECT Lower([Object_Name]) AS Object_Name, [Object_Type]
                                FROM UpdatedMetisDBOS";
                return connection.Query<MetisDBOS>(sql).ToList();
            }
        }

        public List<string> GetAllViews()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT LOWER(Name) AS Name
                                FROM sys.Views";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetAllTables()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT LOWER(Name) AS Name
                                FROM sys.Tables";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetAllStoredProcedures()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT LOWER(Name) AS Name
                                FROM sysobjects WHERE type='P'";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<string> GetAllFunctions()
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT LOWER(Name) AS Name
                                FROM sys.sql_modules modules 
                                INNER JOIN sys.objects objects
                                ON modules.object_id=objects.object_id
                                WHERE type_desc like '%function%'";
                return connection.Query<string>(sql).ToList();
            }
        }

        public List<MetisDBOS> GetViewsObjectDefinition(List<string> viewNames)
        {
            var parameters = new { viewNames };
            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT Lower(Name) as Object_Name, Object_Definition(object_id) AS Object_Type
                                FROM sys.Views 
                                WHERE Name in @viewNames";
                return connection.Query<MetisDBOS>(sql, parameters).ToList();
            }
        }

        public List<MetisDBOS> GetSpsFunctionsObjectDefinition(List<string> spNames)
        {
            var parameters = new { spNames };
            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT Lower(Name) as Object_Name, Object_Definition(id) AS Object_Type
                                FROM sysobjects 
                                WHERE Name in @spNames";
                return connection.Query<MetisDBOS>(sql, parameters).ToList();
            }
        }
        public string IsViewBroken(string view)
        {
            var parameters = new { view };
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection").Replace("60", "15")))
                {
                    string sql = "SELECT DISTINCT '' FROM " + view;
                    connection.Execute(sql, parameters);
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        public bool PicklistsHyrdrated()
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
                {
                    string sql = "SELECT count(*) FROM picklists";
                    int count = connection.QueryFirst<int>(sql);
                    if (count > 0) return true;
                    else return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string GetTriggerObjectDefinition(string triggerName)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("MetisConnection")))
            {
                string sql = $@"SELECT definition FROM sys.sql_modules
                                WHERE object_id = object_id('{triggerName}')";
                return connection.QueryFirst<string>(sql);
            }



        }
    }
}
