using DBMigration.Models;

namespace DBMigration.Repositories.Interfaces
{
    public interface IMetisRepository
    {
        List<MetisDBOS> GetAllNewMetisDBOS();
        List<MetisDBOS> GetExpectedUpdatedDBOS();
        List<string> GetAllViews();
        List<string> GetAllTables();
        List<string> GetAllFunctions();
        List<string> GetAllStoredProcedures();
        List<MetisDBOS> GetViewsObjectDefinition(List<string> viewNames);
        List<MetisDBOS> GetSpsFunctionsObjectDefinition(List<string> spNames);
        string IsViewBroken(string view);
        bool PicklistsHyrdrated();
        string GetTriggerObjectDefinition(string triggerName);

    }
}
