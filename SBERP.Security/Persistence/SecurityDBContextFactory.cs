using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SBERP.Security.Persistence
{
    /// <summary>
    /// Provides the manual configuration the Add-Migration / Update-Database
    /// commands need. Design-time runs OUTSIDE Program.cs, so it loads its own
    /// configuration and, when IsConnectionStrInAzureVault is true, pulls the
    /// connection string from the same Azure Key Vault the runtime uses. Falls
    /// back to the local default so migrations still work offline.
    ///
    /// Reads AppSettings:ConnectionStrings:SCSqlConnectionString (vault-supplied,
    /// secret name "AppSettings--ConnectionStrings--SCSqlConnectionString"),
    /// falling back to AppSettings:ConnectionStrings:SCDefaultConnectionString.
    ///
    /// NOTE: the previous version read "ProdSqlConnectionString", which does not
    /// exist in appsettings.json — corrected to SCSqlConnectionString to match
    /// the runtime app and your AppSettings:ConnectionStrings section.
    /// </summary>
    public class SecurityDBContextFactory : IDesignTimeDbContextFactory<SecurityDBContext>
    {
        public SecurityDBContext CreateDbContext(string[] args)
        {
            // Path Fix: Goes up three levels from the /bin/Debug/net9.0/ folder
            // to the SBERP.Security project root where appsettings.json is located.
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "");

            // 1. Load Configuration from the project root
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = configBuilder.Build();

            // 2. Add Key Vault as a source when enabled (mirrors Program.cs).
            //    Added last so vault secrets override appsettings.json. The
            //    provider maps "--" in secret names to ":".
            bool useVault = bool.Parse(configuration["IsConnectionStrInAzureVault"] ?? "false");
            var keyVaultUri = configuration["KeyVaultUri"];

            if (useVault && !string.IsNullOrWhiteSpace(keyVaultUri))
            {
                configBuilder.AddAzureKeyVault(
                    new Uri(keyVaultUri),
                    new DefaultAzureCredential());
                configuration = configBuilder.Build();   // rebuild so the vault values are present
            }

            // 3. Read from the SAME path the runtime app uses; fall back to the local default when the vault value is empty/absent.
            var connectionString = configuration.GetSection("AppSettings:ConnectionStrings")["SCSqlConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = configuration.GetSection("AppSettings:ConnectionStrings")["SCSqlConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "No Security connection string for design-time. Set the Key Vault secret " +
                    "'AppSettings--ConnectionStrings--SCSqlConnectionString' or the " +
                    "'AppSettings:ConnectionStrings:SCDefaultConnectionString' fallback. " +
                    $"BasePath checked: {basePath}");

            // 4. Configure DbContext using SQL Server
            var optionsBuilder = new DbContextOptionsBuilder<SecurityDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 5. Return the configured DbContext
            return new SecurityDBContext(optionsBuilder.Options);
        }
    }
}
