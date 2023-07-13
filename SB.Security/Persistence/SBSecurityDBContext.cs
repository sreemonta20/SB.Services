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

        
        public virtual DbSet<UserInfo>? UserInfo { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserRole>? UserRole { get; set; }
        public virtual DbSet<UserMenu>? UserMenu { get; set; }
        public virtual DbSet<UserRoleMenu>? UserRoleMenu { get; set; }

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
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });
            modelBuilder.Entity<UserRole>().HasData(
                  new UserRole()
                  {
                      Id = Guid.NewGuid(),
                      RoleName = ConstantSupplier.ADMIN,
                      Description = ConstantSupplier.ADMIN,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRole() 
                  {
                      Id = Guid.NewGuid(),
                      RoleName = ConstantSupplier.USER,
                      Description = ConstantSupplier.USER,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<UserMenu>().HasData(
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name= "Home",
                      IsHeader = true,
                      CssClass = "nav-header",
                      RouteLink = "",
                      RouteLinkClass = "",
                      Icon = "",
                      Remark = "Header",
                      ParentId = null,
                      DropdownIcon = null,
                      SerialNo = 1,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Dashboard",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/home",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-tachometer-alt",
                      Remark = "Navigation Item",
                      ParentId = null,
                      DropdownIcon = null,
                      SerialNo = 2,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Business",
                      IsHeader = true,
                      CssClass = "nav-header",
                      RouteLink = "",
                      RouteLinkClass = "",
                      Icon = "",
                      Remark = "Header",
                      ParentId = null,
                      DropdownIcon = null,
                      SerialNo = 3,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Security",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-cog",
                      Remark = "Navigation Item",
                      ParentId = Guid.NewGuid(),
                      DropdownIcon = "fas fa-angle-left right",
                      SerialNo = 4,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "User",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/security",
                      RouteLinkClass = "nav-link",
                      Icon = "far fa-circle nav-icon",
                      Remark = "Navigation Item",
                      ParentId = Guid.NewGuid(),
                      DropdownIcon = null,
                      SerialNo = 5,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Settings",
                      IsHeader = true,
                      CssClass = "nav-header",
                      RouteLink = "",
                      RouteLinkClass = "",
                      Icon = "",
                      Remark = "Header",
                      ParentId = null,
                      DropdownIcon = null,
                      SerialNo = 6,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserMenu()
                  {
                      Id = Guid.NewGuid(),
                      Name = "App Settings",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/appsettings",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-cog",
                      Remark = "Navigation Item",
                      ParentId = null,
                      DropdownIcon = null,
                      SerialNo = 7,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<UserRoleMenu>().HasData(
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = Guid.NewGuid(),
                      MenuId = Guid.NewGuid(),
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = Guid.NewGuid(),
                      MenuId = Guid.NewGuid(),
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

        }
    }
}
