using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SBERP.HumanResources.Persistence
{
    /// <summary>
    /// Required by `dotnet ef migrations add` / `Update-Database`.
    /// Design-time runs OUTSIDE Program.cs, so the Key Vault wiring there doesn't
    /// apply — this factory loads its own configuration and, when
    /// IsConnectionStrInAzureVault is true, pulls the connection string from the
    /// same Azure Key Vault the runtime uses. Falls back to the local default so
    /// migrations still work offline.
    ///
    /// Reads AppSettings:ConnectionStrings:HRSqlConnectionString (vault-supplied,
    /// secret name "AppSettings--ConnectionStrings--HRSqlConnectionString"),
    /// falling back to AppSettings:ConnectionStrings:HRDefaultConnectionString.
    /// </summary>
    public class HumanResourcesDBContextFactory
        : IDesignTimeDbContextFactory<HumanResourcesDBContext>
    {
        public HumanResourcesDBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();

            // 1. Base configuration from the project root
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

            // 3. Read from the SAME path the runtime app uses; fall back to the
            //    local default when the vault value is empty/absent.
            var section = configuration.GetSection("AppSettings:ConnectionStrings");
            var cs = section["HRSqlConnectionString"];
            if (string.IsNullOrWhiteSpace(cs))
                cs = section["HRDefaultConnectionString"];

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "No HR connection string for design-time. Set the Key Vault secret " +
                    "'AppSettings--ConnectionStrings--HRSqlConnectionString' or the " +
                    "'AppSettings:ConnectionStrings:HRDefaultConnectionString' fallback. " +
                    $"BasePath checked: {basePath}");

            // 4. Configure DbContext using SQL Server
            var builder = new DbContextOptionsBuilder<HumanResourcesDBContext>();
            builder.UseSqlServer(cs);

            // 5. Return the configured DbContext
            return new HumanResourcesDBContext(builder.Options);
        }
    }
}
