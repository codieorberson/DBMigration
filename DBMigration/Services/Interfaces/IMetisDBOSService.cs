using System.Data;
namespace DBMigration.Services.Interfaces
{
    public interface IMetisDBOSService
    {
        DataTable NewMetisDBOSExist();
        DataTable MetisDBOSUpdated();
        DataTable BrokenMetisViews();
        DataTable PicklistsTableHyrdrated();
        DataTable SnapLogicRelatedDBOSUpdated();
    }
}