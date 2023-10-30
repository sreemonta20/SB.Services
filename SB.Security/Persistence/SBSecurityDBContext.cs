using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SB.Security.Models.Base;
using SB.Security.Helper;
using StackExchange.Redis;

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

        public virtual DbSet<UserRole>? UserRole { get; set; }
        public virtual DbSet<UserMenu>? UserMenu { get; set; }
        public virtual DbSet<UserRoleMenu>? UserRoleMenu { get; set; }
        public virtual DbSet<UserInfo>? UserInfo { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region With refresh seeding
            //modelBuilder.Entity<UserRole>().HasData(
            //      new UserRole()
            //      {
            //          Id = Guid.NewGuid(),
            //          RoleName = ConstantSupplier.ADMIN,
            //          Description = ConstantSupplier.ADMIN,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserRole()
            //      {
            //          Id = Guid.NewGuid(),
            //          RoleName = ConstantSupplier.USER,
            //          Description = ConstantSupplier.USER,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<UserMenu>().HasData(
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "Home",
            //          IsHeader = true,
            //          CssClass = "nav-header",
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 1,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "Dashboard",
            //          IsHeader = false,
            //          CssClass = "nav-item",
            //          RouteLink = "/business/home",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-tachometer-alt",
            //          Remark = "Navigation Item",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 2,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "Business",
            //          IsHeader = true,
            //          CssClass = "nav-header",
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 3,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "Security",
            //          IsHeader = false,
            //          CssClass = "nav-item",
            //          RouteLink = "",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-cog",
            //          Remark = "Navigation Item",
            //          ParentId = null,
            //          DropdownIcon = "fas fa-angle-left right",
            //          SerialNo = 4,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "User",
            //          IsHeader = false,
            //          CssClass = "nav-item",
            //          RouteLink = "/business/security",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = Guid.NewGuid(),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "Settings",
            //          IsHeader = true,
            //          CssClass = "nav-header",
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 6,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          Name = "App Settings",
            //          IsHeader = false,
            //          CssClass = "nav-item",
            //          RouteLink = "/business/appsettings",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-cog",
            //          Remark = "Navigation Item",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 7,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<UserRoleMenu>().HasData(
            //      new UserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          RoleId = Guid.NewGuid(),
            //          MenuId = Guid.NewGuid(),
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          RoleId = Guid.NewGuid(),
            //          MenuId = Guid.NewGuid(),
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<UserInfo>().HasData(
            //      new UserInfo()
            //      {
            //          Id = Guid.NewGuid(),
            //          FullName = "Sreemonta Bhowmik",
            //          UserName = "sree",
            //          Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
            //          SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
            //          Email = "sbhowmikcse08@gmail.com",
            //          RoleId = Guid.NewGuid(),
            //          LastLoginAttemptAt = DateTime.Now,
            //          LoginFailedAttemptsCount = 0,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new UserInfo()
            //      {
            //          Id = Guid.NewGuid(),
            //          FullName = "Anannya Rohine",
            //          UserName = "rohine",
            //          Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
            //          SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
            //          Email = "rohine2008@gmail.com",
            //          RoleId = Guid.NewGuid(),
            //          LastLoginAttemptAt = DateTime.Now,
            //          LoginFailedAttemptsCount = 0,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });
            #endregion

            modelBuilder.Entity<UserRole>().HasData(
                  new UserRole()
                  {
                      Id = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
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
                      Id = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
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
                      Id = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                      Name = "Home",
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
                      Id = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
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
                      Id = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
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
                      Id = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                      Name = "Security",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-cog",
                      Remark = "Navigation Item",
                      ParentId = null,
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
                      Id = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                      Name = "User",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/security",
                      RouteLinkClass = "nav-link",
                      Icon = "far fa-circle nav-icon",
                      Remark = "Navigation Item",
                      ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
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
                      Id = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
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
                      Id = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
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
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      MenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  }, 
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      MenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      RoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      MenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                      IsView = true,
                      IsCreate = false,
                      IsUpdate = false,
                      IsDelete = false,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<AppUserProfile>().HasData(
                  new UserInfo()
                  {
                      Id = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
                      FullName = "Sreemonta Bhowmik",
                      UserName = "sree",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      Email = "sbhowmikcse08@gmail.com",
                      RoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      LastLoginAttemptAt = DateTime.Now,
                      LoginFailedAttemptsCount = 0,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new UserInfo()
                  {
                      Id = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
                      FullName = "Anannya Rohine",
                      UserName = "rohine",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      Email = "rohine2008@gmail.com",
                      RoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      LastLoginAttemptAt = DateTime.Now,
                      LoginFailedAttemptsCount = 0,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

        }
    }
}
