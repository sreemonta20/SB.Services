namespace SB.Security.Models.Configuration
{
    /// <summary>
    /// It is to track the Security connection in  <see cref="AppSettings"/>.
    /// </summary>
    public class ConnectionStrings
    {
        public string? PrimaryConnectionString { get; set; }
        public string? SecondaryConnectionString { get; set; }
    }
}