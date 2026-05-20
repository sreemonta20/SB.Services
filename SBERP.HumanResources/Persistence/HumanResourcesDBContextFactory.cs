using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SBERP.HumanResources.Persistence
{
    /// <summary>
    /// Required by `dotnet ef migrations add` / `Update-Database`.
    /// Reads connection string from appsettings.json's ConnectionStrings:HRDatabase.
    /// </summary>
    public class HumanResourcesDBContextFactory
        : IDesignTimeDbContextFactory<HumanResourcesDBContext>
    {
        public HumanResourcesDBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var cs = configuration.GetSection("ConnectionStrings")["HRDatabase"];
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "ConnectionStrings:HRDatabase missing in appsettings.json. " +
                    $"BasePath checked: {basePath}");

            var builder = new DbContextOptionsBuilder<HumanResourcesDBContext>();
            builder.UseSqlServer(cs);
            return new HumanResourcesDBContext(builder.Options);
        }
    }
}
