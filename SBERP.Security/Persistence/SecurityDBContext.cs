using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBERP.Security.Models.Base;
using SBERP.Security.Helper;
using StackExchange.Redis;
using FluentEmail.Core.Models;
using SSBERP.Security.Models.Base;

namespace SBERP.Security.Persistence
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
        public virtual DbSet<AppUserRoleLog>? AppUserRoleLog { get; set; }
        public virtual DbSet<AppUserProfile>? AppUserProfiles { get; set; }
        public virtual DbSet<AppUserProfileLog>? AppUserProfileLog { get; set; }
        public virtual DbSet<AppUser>? AppUsers { get; set; }
        public virtual DbSet<AppUserLog>? AppUserLog { get; set; }
        public virtual DbSet<AppUserMenu>? AppUserMenus { get; set; }
        public virtual DbSet<AppUserMenuLog>? AppUserMenuLog { get; set; }
        public virtual DbSet<AppUserRoleMenu>? AppUserRoleMenus { get; set; }
        public virtual DbSet<AppUserRoleMenuLog>? AppUserRoleMenuLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary keys
            modelBuilder.Entity<AppUserRole>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserRoleLog>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserProfile>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserProfileLog>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUser>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserLog>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserMenu>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserMenuLog>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserRoleMenu>().HasKey(x => x.Id);
            modelBuilder.Entity<AppUserRoleMenuLog>().HasKey(x => x.Id);


            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.RoleName).HasMaxLength(50);
                entity.Property(x => x.Description).HasMaxLength(100);
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoles"));
                entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoles"));
                entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoles"));
            });

            modelBuilder.Entity<AppUserRoleLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
                entity.Property(x => x.RoleName).HasMaxLength(50);
                entity.Property(x => x.Description).HasMaxLength(100);
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
                entity.Property(x => x.Action).HasColumnName("Action");
            });

            modelBuilder.Entity<AppUserMenu>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.Name).HasMaxLength(100);
                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
                entity.Property(x => x.IsModule).HasColumnName("IsModule");
                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
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
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserMenus"));
                entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserMenus"));
                entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserMenus"));
            });

            modelBuilder.Entity<AppUserMenuLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
                entity.Property(x => x.Name).HasMaxLength(100);
                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
                entity.Property(x => x.IsModule).HasColumnName("IsModule");
                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
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
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
                entity.Property(x => x.Action).HasColumnName("Action");
            });

            modelBuilder.Entity<AppUserProfile>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.FullName).HasMaxLength(200);
                entity.Property(x => x.Address).HasMaxLength(200); 
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
                entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserProfiles"));
                entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserProfiles"));
                entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserProfiles"));
            });

            modelBuilder.Entity<AppUserProfileLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
                entity.Property(x => x.FullName).HasMaxLength(200);
                entity.Property(x => x.Address).HasMaxLength(200);
                entity.Property(x => x.Email).HasMaxLength(200);
                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
                entity.Property(x => x.Action).HasColumnName("Action");
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
                    .HasForeignKey(x => x.AppUserMenuId)
                    .HasConstraintName("FK_AppUserRoleMenus_AppUserMenu");
                entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoleMenus"));
                entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoleMenus"));
                entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoleMenus"));
            });

            modelBuilder.Entity<AppUserRoleMenuLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserRoleMenuId).HasColumnName("AppUserRoleMenuId");
                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
                entity.Property(x => x.IsView).HasColumnName("IsView");
                entity.Property(x => x.IsCreate).HasColumnName("IsCreate");
                entity.Property(x => x.IsUpdate).HasColumnName("IsUpdate");
                entity.Property(x => x.IsDelete).HasColumnName("IsDelete");
                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
                entity.Property(x => x.Action).HasColumnName("Action");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
                entity.Property(x => x.UserName).HasMaxLength(100);
                entity.Property(x => x.Password).HasMaxLength(255);
                entity.Property(x => x.SaltKey).HasMaxLength(255);
                entity.Property(x => x.RefreshToken).HasMaxLength(255);
                entity.Property(x => x.RefreshTokenExpiryTime).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.HasOne(x => x.AppUserProfile)
                    .WithOne(p => p.AppUser)
                    .HasForeignKey<AppUser>(x => x.AppUserProfileId)
                    .HasConstraintName("FK_AppUser_AppUserProfiles");
                entity.ToTable(x => x.HasTrigger("TRG_InsertAppUsers"));
                entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUsers"));
                entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUsers"));
            });

            modelBuilder.Entity<AppUserLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserId).HasColumnName("AppUserId");
                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
                entity.Property(x => x.UserName).HasMaxLength(100);
                entity.Property(x => x.Password).HasMaxLength(255);
                entity.Property(x => x.SaltKey).HasMaxLength(255);
                entity.Property(x => x.RefreshToken).HasMaxLength(255);
                entity.Property(x => x.RefreshTokenExpiryTime).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");
                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
                entity.Property(x => x.Action).HasColumnName("Action");
            });

            modelBuilder.Entity<AppUserRole>().HasData(
                  new AppUserRole()
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
                  new AppUserRole()
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
                      Address = "Dubai",
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
                      Address = "Dubai",
                      Email = "rohine2008@gmail.com",
                      AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                      CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  });

            modelBuilder.Entity<AppUser>().HasData(
                  new AppUser()
                  {
                      Id = new Guid("5FD67AAF-183E-48F8-BB53-15B7628A3E0A"),
                      AppUserProfileId = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
                      UserName = "sree",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      RefreshToken = null,
                      RefreshTokenExpiryTime = null,
                      CreatedBy = null,
                      CreatedDate = DateTime.UtcNow,
                      UpdatedBy = null,
                      UpdatedDate = DateTime.UtcNow,
                      IsActive = true
                  },
                  new AppUser()
                  {
                      Id = new Guid("A9E00F47-89EC-46A3-A5E8-31EBD52AC121"),
                      AppUserProfileId = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
                      UserName = "rohine",
                      Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                      SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                      RefreshToken = null,
                      RefreshTokenExpiryTime = null,
                      CreatedBy = null,
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
                      IsModule = false,
                      IsComponent = false,
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
                      IsModule = true,
                      IsComponent = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/home",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-tachometer-alt",
                      Remark = "Navigation Item",
                      ParentId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
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
                      IsModule = false,
                      IsComponent = false,
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
                      IsModule = true,
                      IsComponent = false,
                      CssClass = "nav-item",
                      RouteLink = "",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-cog",
                      Remark = "Navigation Item",
                      ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
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
                      Name = "User Role",
                      IsHeader = false,
                      IsModule = false,
                      IsComponent = true,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/appuserrole",
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
                      Id = new Guid("15F1D22F-6DA5-4ED7-9518-C0B85F7014AD"),
                      Name = "User Menu",
                      IsHeader = false,
                      IsModule = false,
                      IsComponent = true,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/appusermenu",
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
                      Id = new Guid("1943AA7C-B50A-484D-9CAD-28456A20C2CC"),
                      Name = "User Role Menu",
                      IsHeader = false,
                      IsModule = false,
                      IsComponent = true,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/appuserrolemenu",
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
                      Id = new Guid("73CC3330-54E9-4152-9ED4-F7C31748985E"),
                      Name = "User Profile",
                      IsHeader = false,
                      IsModule = false,
                      IsComponent = true,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/appuserprofile",
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
                      Id = new Guid("AD0983EB-F60C-4329-8516-1650537A0567"),
                      Name = "Application User",
                      IsHeader = false,
                      IsModule = false,
                      IsComponent = true,
                      CssClass = "nav-item",
                      RouteLink = "/business/security/appuser",
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
                      IsModule = false,
                      IsComponent = false,
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
                      IsModule = true,
                      IsComponent = false,
                      CssClass = "nav-item",
                      RouteLink = "/business/appsettings",
                      RouteLinkClass = "nav-link active",
                      Icon = "nav-icon fas fa-cog",
                      Remark = "Navigation Item",
                      ParentId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
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
