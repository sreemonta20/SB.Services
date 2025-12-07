using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SBERP.Security.Persistence
{
    // This factory provides the manual configuration needed by the Add-Migration command
    public class SecurityDBContextFactory : IDesignTimeDbContextFactory<SecurityDBContext>
    {
        public SecurityDBContext CreateDbContext(string[] args)
        {
            // Path Fix: Goes up three levels from the /bin/Debug/net9.0/ folder
            // to the SBERP.Security project root where appsettings.json is located.
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "");

            // 1. Load Configuration from the project root
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Extract the connection string from the verified nested path
            var connectionString = configuration.GetSection("AppSettings:ConnectionStrings")["ProdSqlConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"ERROR: Could not find 'ProdSqlConnectionString' in appsettings.json. Path checked: {basePath}");
            }

            // 3. Configure DbContext using SQL Server
            var optionsBuilder = new DbContextOptionsBuilder<SecurityDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 4. Return the configured DbContext
            return new SecurityDBContext(optionsBuilder.Options);
        }
    }
}
