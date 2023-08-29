using System.Data;
using DBMigration.Services;
using DBMigration.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBMigration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Verify : ControllerBase
    {
        IDBUsersService dbUsersService;
        IIceUserPermissionsService iceUserPermissionsService;
        IIceTablesRefreshedService iceTablesRefreshedService;
        IMetisDBOSService metisDBOSService;
        IDBOSService dbosService;
        IExcelService excelService;
        DataSet dataSet;
        private string fileTimeStamp;

        public Verify(IDBUsersService dbUsersService, IIceUserPermissionsService iceUserPermissionsService, IIceTablesRefreshedService iceTablesRefreshedService, IMetisDBOSService metisDBOSService, IDBOSService dbosService, IExcelService excelService)
        {
            this.dbUsersService = dbUsersService;
            this.iceUserPermissionsService = iceUserPermissionsService;
            this.iceTablesRefreshedService = iceTablesRefreshedService;
            this.metisDBOSService = metisDBOSService;
            this.dbosService = dbosService;
            this.excelService = excelService;
            this.dataSet = new DataSet();
            fileTimeStamp = (DateTime.Now.ToString("MMMM dd") + " " + DateTime.Now.ToString("h:mm tt")).Replace(" ", "-").Replace(":", "-");
        }

        [Route("IceUserPermissionsExist")]
        [HttpGet]
        public void IceUserPermissionsExist()
        {
            var dataTable = iceUserPermissionsService.IceUserPermissionsExist();
            this.dataSet = new DataSet("MissingIceUserPermission");
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("NewMetisDBOSExist")]
        [HttpGet]
        public void NewMetisDBOSExist()
        {
            var dataTable = metisDBOSService.NewMetisDBOSExist();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("DBOSRefSaleforceUpdated")]
        [HttpGet]
        public void DBOSRefSaleforceUpdated()
        {
            var dataTable = metisDBOSService.MetisDBOSUpdated();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("DBOSExist")]
        [HttpGet]
        public void DBOSExist()
        {
            var ds = dbosService.DBOSExist();
            this.dataSet = new DataSet("DBOSExist");
            foreach (DataTable t in ds.Tables)
            {
                t.TableName = "DBOSExist-" + t.TableName;

                this.dataSet.Tables.Add(t.Copy());
            }
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("DBUsersExist")]
        [HttpGet]
        public void DBUsersExist()
        {
            var dataTable = dbUsersService.DBUsersExist();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("DBUsersPermissionsExist")]
        [HttpGet]
        public void DBUsersPermissionsExist()
        {
            var dataTable = dbUsersService.DBUsersPermissionsExist();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        

        [Route("IceTablesRoutinelyRefreshed")]
        [HttpGet]
        public void IceTablesUpdated()
        {
            var dataTable = iceTablesRefreshedService.TablesRefreshed();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }


        [Route("MetisViewsBroken")]
        [HttpGet]
        public void MetisViewsFunctioning()
        {
            var dataTable = metisDBOSService.BrokenMetisViews();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("LinkedServersFunctioning")]
        [HttpGet]
        public void LinkedServersFunctioning()
        {
            var dataTable = dbosService.LinkedServersFunctioning();
            this.dataSet = new DataSet(dataTable.TableName);
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("PicklistsHydrated")]
        [HttpGet]
        public void PicklistsTableHyrdrated()
        {
            var dataTable = metisDBOSService.PicklistsTableHyrdrated();
            this.dataSet = new DataSet("PicklistTableHyrdrated");
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("SnapLogicRelatedDBOSUpdated")]
        [HttpGet]
        public void AutomicRelatedDBOsUpdated()
        {
            var dataTable = metisDBOSService.SnapLogicRelatedDBOSUpdated();
            this.dataSet = new DataSet("SnapLogicRelatedDBOSUpdated");
            this.dataSet.Tables.Add(dataTable);
            this.excelService.SaveDataSetAsExcel(this.dataSet, $"Reports/{this.dataSet.DataSetName}{fileTimeStamp}.xlsx");
        }

        [Route("RunAllReports")]
        [HttpGet]
        public void RunAllReports()
        {
            List<DataTable> allTables = new List<DataTable>();
            allTables.Add(dbUsersService.DBUsersExist());
            allTables.Add(dbUsersService.DBUsersPermissionsExist());
            allTables.Add(iceUserPermissionsService.IceUserPermissionsExist());
            allTables.Add(metisDBOSService.NewMetisDBOSExist());
            allTables.Add(metisDBOSService.MetisDBOSUpdated());
            allTables.Add(metisDBOSService.BrokenMetisViews());
            allTables.Add(iceTablesRefreshedService.TablesRefreshed());
            allTables.Add(dbosService.LinkedServersFunctioning());
            allTables.Add(metisDBOSService.PicklistsTableHyrdrated());
            allTables.Add(metisDBOSService.SnapLogicRelatedDBOSUpdated());

            DataSet DBOSExist = dbosService.DBOSExist();
            foreach (DataTable t in DBOSExist.Tables)
            {
                t.TableName = "DBOSMissing-" + t.TableName;
                allTables.Add(t.Copy());
            }

            DataSet fullDataSet = new DataSet("FullReport");
            foreach(DataTable t in allTables)
            {
                fullDataSet.Tables.Add(t);
            }

            this.excelService.SaveDataSetAsExcel(fullDataSet, $"Reports/FullReport-{fileTimeStamp}.xlsx");
        }
    }
}