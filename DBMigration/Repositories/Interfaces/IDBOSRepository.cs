using DBMigration.Models;

namespace DBMigration.Repositories.Interfaces
{
    public interface IDBOSRepository
    {
        List<string> GetExpectedLinkedServers();
        string TestLinkedServer(string linkedServerName);
        List<DBOS> GetExpectedDBOS();
        List<Schemas> GetExpectedSchemas();
        List<Indexes> GetExpectedIndexes();
        List<string> GetTables(string database);
        List<string> GetViews(string database);
        List<string> GetFunctions(string database);
        List<string> GetSps(string database);
        
        List<Indexes> GetActualIndexes(string database);
        List<string> GetSchemas(string database);
        string GetObjectDefinition(string database, string objectName);


    }
}
