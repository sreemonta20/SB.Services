namespace SBERP.Security.Models.Configuration
{
    /// <summary>
    /// It is to track the Security connection in  <see cref="AppSettings"/>.
    /// </summary>
    public class ConnectionStrings
    {
        public string? SCSqlConnectionString { get; set; }
        public string? SCOracleConnectionString { get; set; }
        public string? SCOdbcConnectionString { get; set; }
        public string? SCOledbConnectionString { get; set; }
        public string? SCDefaultConnectionString { get; set; }
    }
}