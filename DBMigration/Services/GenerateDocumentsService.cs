using DBMigration.Models;

namespace DBMigration.Services
{
    public class GenerateDocumentsService : IGenerateDocumentsService
    {
        public void DBUsersExist(DBUser dbUser, bool exists, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"{dbUser.User} : {exists}");
                wr.Close();
            }
        }

        public void DBUsersPermissionsExist(string dbUser, string database, bool exists, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"{dbUser} {database} : {exists}");
                wr.Close();
            }
        }

        public void IceUserPermissions(IceUserPermissions missingPermission, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"{missingPermission.Table_Name.ToUpper()} {missingPermission.Column_Name}");
                wr.Close();
            }
        }

        public void IceTablesRefreshed(TableRefreshTime tableRefreshTime, bool refreshed, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"{tableRefreshTime.TblName} Refreshed: {refreshed}");
                wr.Close();
            }
        }

        public void AddMissingDBO(string dbObject, string dbObjectType, string documentPath, string database)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"Missing {database} {dbObjectType} : {dbObject}");
                wr.Close();
            }
        }
        //                generateDocumentsService.LinkedServerFunctioning(expectedLinkedServer, valid, validMessage, $"Reports/LinkedServersFunctioning{fileTimeStamp}.txt");

        public void LinkedServerFunctioning(string linkedServerName, bool valid, string validMessage, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                if (valid) wr.WriteLine($"{linkedServerName} : {valid}");
                else wr.WriteLine($"{linkedServerName} : {valid}, Error: {validMessage}");
                wr.Close();
            }
        }

        public void AddBrokenMetisObject(string metisObject, string metisObjectType, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"Broken Metis {metisObjectType} : {metisObject}");
                wr.Close();
            }
        }

        public void AddMissingMetisObject(string metisObject, string metisObjectType, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"Missing Metis {metisObjectType} : {metisObject}");
                wr.Close();
            }
        }

        public void AddBrokenMetisView(string view, string error, string documentPath)
        {
            using (StreamWriter wr = File.AppendText(documentPath))
            {
                wr.WriteLine($"{view} : {error}");
                wr.Close();
            }
        }

    }
}
