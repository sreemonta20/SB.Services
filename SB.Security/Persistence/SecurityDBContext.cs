﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SB.Security.Models.Base;
using SB.Security.Helper;
using StackExchange.Redis;

namespace SB.Security.Persistence
{
    public class SecurityDBContext : DbContext
    {
        public SecurityDBContext()
        {
        }
        public SecurityDBContext(DbContextOptions<SecurityDBContext> options) : base(options) 
        { 

        }


        public virtual DbSet<AppUserRole>? AppUserRoles { get; set; }
        public virtual DbSet<AppUserProfile>? AppUserProfiles { get; set; }
        public virtual DbSet<AppUserMenu>? AppUserMenus { get; set; }
        public virtual DbSet<AppUserRoleMenu>? AppUserRoleMenus { get; set; }
        public DbSet<AppLoggedInUser> AppLoggedInUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary keys
            modelBuilder.Entity<AppUserRole>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserMenu>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserProfile>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserRoleMenu>().HasKey(x => x.Id);
            modelBuilder.Entity<AppLoggedInUser>().HasKey(x => x.Id);

            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.RoleName).HasMaxLength(50);
                entity.Property(x => x.Description).HasMaxLength(100);
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserMenu>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.Name).HasMaxLength(100);
                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
                entity.Property(x => x.CssClass).HasMaxLength(100);
                entity.Property(x => x.RouteLink).HasMaxLength(255);
                entity.Property(x => x.RouteLinkClass).HasMaxLength(200);
                entity.Property(x => x.Icon).HasMaxLength(100);
                entity.Property(x => x.Remark).HasMaxLength(255);
                entity.Property(x => x.ParentId).HasColumnName("ParentId");
                entity.Property(x => x.DropdownIcon).HasMaxLength(100);
                entity.Property(x => x.SerialNo).HasColumnName("SerialNo");
                entity.Property(x => x.Remark).HasMaxLength(255);
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserProfile>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.FullName).HasMaxLength(200);
                entity.Property(x => x.UserName).HasMaxLength(100);
                entity.Property(x => x.Password).HasMaxLength(255);
                entity.Property(x => x.SaltKey).HasMaxLength(255);
                entity.Property(x => x.Email).HasMaxLength(200);
                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.HasOne(x => x.AppUserRole)
                    .WithMany(p => p.AppUserProfiles)
                    .HasForeignKey(d => d.AppUserRoleId)
                    .HasConstraintName("FK_AppUserProfiles_AppUserRole");
            });

            modelBuilder.Entity<AppUserRoleMenu>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
                entity.Property(x => x.IsView).HasColumnName("IsView");
                entity.Property(x => x.IsCreate).HasColumnName("IsCreate");
                entity.Property(x => x.IsUpdate).HasColumnName("IsUpdate");
                entity.Property(x => x.IsDelete).HasColumnName("IsDelete");
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.HasOne(x => x.AppUserRole)
                    .WithMany(p => p.AppUserRoleMenus)
                    .HasForeignKey(d => d.AppUserRoleId)
                    .HasConstraintName("FK_AppUserRoleMenus_AppUserRole");
                entity.HasOne(x => x.AppUserMenu)
                    .WithMany(x => x.AppUserRoleMenus)
                    .HasForeignKey(x => x.AppUserRoleId)
                    .HasConstraintName("FK_AppUserRoleMenus_AppUserMenu");
            });

            modelBuilder.Entity<AppLoggedInUser>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
                entity.Property(x => x.RefreshToken).HasMaxLength(255);
                entity.Property(x => x.RefreshTokenExpiryTime).HasColumnType("datetime");
                entity.Property(x => x.LastLoginAttemptAt).HasColumnType("datetime");
                entity.Property(x => x.LoginFailedAttemptsCount).HasColumnName("LoginFailedAttemptsCount");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.HasOne(x => x.AppUserProfile)
                    .WithOne(x => x.AppLoggedInUser)
                    .HasForeignKey<AppUserProfile>(x => x.Id);
            });

            modelBuilder.Entity<AppUserRole>().HasData(
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
                      CreatedBy = "1B15CE5A-56B3-4EB9-8286-6E27F770B0DA",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<AppUserProfile>().HasData(
                  new AppUserProfile()
                  {
                      Id = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
                      FullName = "Sreemonta Bhowmik",
                      UserName = "sree",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      Email = "sbhowmikcse08@gmail.com",
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserProfile()
                  {
                      Id = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
                      FullName = "Anannya Rohine",
                      UserName = "rohine",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      Email = "rohine2008@gmail.com",
                      AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<AppUserMenu>().HasData(
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
                  {
                      Id = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                      Name = "User",
                      IsHeader = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/user",
                      RouteLinkClass = "nav-link",
                      Icon = "far fa-circle nav-icon",
                      Remark = "Navigation Item",
                      ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                      DropdownIcon = null,
                      SerialNo = 5,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserMenu()
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
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<AppUserRoleMenu>().HasData(
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                      AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
                      IsView = true,
                      IsCreate = true,
                      IsUpdate = true,
                      IsDelete = true,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                      IsView = null,
                      IsCreate = null,
                      IsUpdate = null,
                      IsDelete = null,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUserRoleMenu()
                  {
                      Id = Guid.NewGuid(),
                      AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                      IsView = true,
                      IsCreate = false,
                      IsUpdate = false,
                      IsDelete = false,
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

        }
    }
}