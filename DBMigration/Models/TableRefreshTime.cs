namespace DBMigration.Models
{
    public class TableRefreshTime
    {
        public string TblName { get; set; }
        public DateTime LastRefreshTime { get; set; }
    }
}
