namespace DBMigration.Models
{
    public class DBUser
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public List<string> Databases { get; set; }
    }
}
