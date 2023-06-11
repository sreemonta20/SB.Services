using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SB.Security.Models.Base;
using SB.Security.Helper;

namespace SB.Security.Persistence
{
    /// <summary>
    /// HCSSecurityDBContext is the main interactive class for entity framework. Code first approach is used.
    /// </summary>
    public class SBSecurityDBContext : DbContext
    {
        // Add-Migration SeedingData
        // update-database
        public SBSecurityDBContext(DbContextOptions options)
            : base(options)
        {
        }

        
        public virtual DbSet<UserInfo>? UserInfos { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserInfo>().HasData(
                  new UserInfo()
                  {
                      Id = Guid.NewGuid(),
                      FullName = "Sreemonta Bhowmik",
                      UserName = "sree",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey  = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      Email = "sbhowmikcse08@gmail.com",
                      UserRole = ConstantSupplier.ADMIN,
                      LastLoginAttemptAt = DateTime.Now,
                      LoginFailedAttemptsCount = 0,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow
                  });


        }
    }
}
