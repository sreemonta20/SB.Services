namespace SBERP.Security.Models.Configuration
{
    /// <summary>
    /// It is to track the Security connection in  <see cref="AppSettings"/>.
    /// </summary>
    public class ConnectionStrings
    {
        public string? ProdSqlConnectionString { get; set; }
        public string? DevSqlConnectionString { get; set; }
        public string? ProdOracleConnectionString { get; set; }
        public string? DevOracleConnectionString { get; set; }
        public string? ProdOdbcConnectionString { get; set; }
        public string? DevOdbcConnectionString { get; set; }
        public string? ProdOledbConnectionString { get; set; }
        public string? DevOledbConnectionString { get; set; }
        public string? ProdBackupConnectionString { get; set; }
        public string? DevBackupConnectionString { get; set; }
    }
}