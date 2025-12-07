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
        public virtual DbSet<AppUserRoleLog>? AppUserRolesLog { get; set; }
        public virtual DbSet<AppUserProfile>? AppUserProfiles { get; set; }
        public virtual DbSet<AppUserProfileLog>? AppUserProfilesLog { get; set; }
        public virtual DbSet<AppUser>? AppUsers { get; set; }
        public virtual DbSet<AppUserLog>? AppUsersLog { get; set; }
        public virtual DbSet<AppUserMenu>? AppUserMenus { get; set; }
        public virtual DbSet<AppUserMenuLog>? AppUserMenusLog { get; set; }
        public virtual DbSet<AppUserRoleMenu>? AppUserRoleMenus { get; set; }
        public virtual DbSet<AppUserRoleMenuLog>? AppUserRoleMenusLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ------------------------------------------------------------------
            // Primary keys
            // ------------------------------------------------------------------
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

            // ------------------------------------------------------------------
            // AppUserRole
            // ------------------------------------------------------------------
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
                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoles"));
                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoles"));
                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoles"));
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUserRoles");
                    table.HasTrigger("TRG_UpdateAppUserRoles");
                    table.HasTrigger("TRG_DeleteAppUserRoles");
                });
            });

            // ------------------------------------------------------------------
            // AppUserRoleLog
            // ------------------------------------------------------------------
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

            // ------------------------------------------------------------------
            // AppUserMenu
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserMenu>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");

                entity.Property(x => x.Name).HasMaxLength(100);
                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
                entity.Property(x => x.IsModule).HasColumnName("IsModule");
                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
                entity.Property(x => x.CssClass).HasMaxLength(100);
                entity.Property(x => x.IsRouteLink).HasColumnName("IsRouteLink");
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
                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserMenus"));
                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserMenus"));
                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserMenus"));
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUserMenus");
                    table.HasTrigger("TRG_UpdateAppUserMenus");
                    table.HasTrigger("TRG_DeleteAppUserMenus");
                });
            });

            // ------------------------------------------------------------------
            // AppUserMenuLog
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserMenuLog>(entity =>
            {
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
                entity.Property(x => x.Name).HasMaxLength(100);
                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
                entity.Property(x => x.IsModule).HasColumnName("IsModule");
                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
                entity.Property(x => x.CssClass).HasMaxLength(100);
                entity.Property(x => x.IsRouteLink).HasColumnName("IsRouteLink");
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

            // ------------------------------------------------------------------
            // AppUserProfile
            // ------------------------------------------------------------------
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
                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserProfiles"));
                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserProfiles"));
                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserProfiles"));
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUserProfiles");
                    table.HasTrigger("TRG_UpdateAppUserProfiles");
                    table.HasTrigger("TRG_DeleteAppUserProfiles");
                });
            });

            // ------------------------------------------------------------------
            // AppUserProfileLog
            // ------------------------------------------------------------------
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

            // ------------------------------------------------------------------
            // AppUserRoleMenu
            // ------------------------------------------------------------------
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

                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoleMenus"));
                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoleMenus"));
                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoleMenus"));
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUserRoleMenus");
                    table.HasTrigger("TRG_UpdateAppUserRoleMenus");
                    table.HasTrigger("TRG_DeleteAppUserRoleMenus");
                });
            });

            // ------------------------------------------------------------------
            // AppUserRoleMenuLog
            // ------------------------------------------------------------------
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

            // ------------------------------------------------------------------
            // AppUser
            // ------------------------------------------------------------------
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

                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUsers"));
                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUsers"));
                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUsers"));
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUsers");
                    table.HasTrigger("TRG_UpdateAppUsers");
                    table.HasTrigger("TRG_DeleteAppUsers");
                });
            });

            // ------------------------------------------------------------------
            // AppUserLog
            // ------------------------------------------------------------------
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

            // ==================================================================
            // SEED DATA  (all deterministic – no DateTime.UtcNow / Guid.NewGuid)
            // ==================================================================
            var roleTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var profileTime = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);
            var userTime = new DateTime(2024, 1, 1, 10, 10, 0, DateTimeKind.Utc);
            var menuTime = new DateTime(2024, 1, 1, 10, 15, 0, DateTimeKind.Utc);
            var roleMenuTime = new DateTime(2024, 1, 1, 10, 20, 0, DateTimeKind.Utc);

            // ------------------------------------------------------------------
            // Seed: AppUserRole
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserRole>().HasData(
                new AppUserRole
                {
                    Id = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    RoleName = ConstantSupplier.ADMIN,
                    Description = ConstantSupplier.ADMIN,
                    CreatedBy = null,
                    CreatedDate = roleTime,
                    UpdatedBy = null,
                    UpdatedDate = roleTime,
                    IsActive = true
                },
                new AppUserRole
                {
                    Id = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    RoleName = ConstantSupplier.USER,
                    Description = ConstantSupplier.USER,
                    CreatedBy = "1B15CE5A-56B3-4EB9-8286-6E27F770B0DA",
                    CreatedDate = roleTime,
                    UpdatedBy = null,
                    UpdatedDate = roleTime,
                    IsActive = true
                });


            // ------------------------------------------------------------------
            // Seed: AppUserProfile
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserProfile>().HasData(
                new AppUserProfile
                {
                    Id = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
                    FullName = "Sreemonta Bhowmik",
                    Address = "Dubai",
                    Email = "sbhowmikcse08@gmail.com",
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    CreatedBy = null,
                    CreatedDate = profileTime,
                    UpdatedBy = null,
                    UpdatedDate = profileTime,
                    IsActive = true
                },
                new AppUserProfile
                {
                    Id = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
                    FullName = "Anannya Rohine",
                    Address = "Dubai",
                    Email = "rohine2008@gmail.com",
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = profileTime,
                    UpdatedBy = null,
                    UpdatedDate = profileTime,
                    IsActive = true
                });

            // ------------------------------------------------------------------
            // Seed: AppUser
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = new Guid("5FD67AAF-183E-48F8-BB53-15B7628A3E0A"),
                    AppUserProfileId = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
                    UserName = "sree",
                    Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                    SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                    RefreshToken = null,
                    RefreshTokenExpiryTime = null,
                    CreatedBy = null,
                    CreatedDate = userTime,
                    UpdatedBy = null,
                    UpdatedDate = userTime,
                    IsActive = true
                },
                new AppUser
                {
                    Id = new Guid("A9E00F47-89EC-46A3-A5E8-31EBD52AC121"),
                    AppUserProfileId = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
                    UserName = "rohine",
                    Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                    SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                    RefreshToken = null,
                    RefreshTokenExpiryTime = null,
                    CreatedBy = null,
                    CreatedDate = userTime,
                    UpdatedBy = null,
                    UpdatedDate = userTime,
                    IsActive = true
                });

            // ------------------------------------------------------------------
            // Seed: AppUserMenu
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserMenu>().HasData(
                new AppUserMenu
                {
                    Id = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                    Name = "Home",
                    IsHeader = true,
                    IsModule = false,
                    IsComponent = false,
                    CssClass = "nav-header",
                    IsRouteLink = false,
                    RouteLink = "",
                    RouteLinkClass = "",
                    Icon = "",
                    Remark = "Header",
                    ParentId = null,
                    DropdownIcon = null,
                    SerialNo = 1,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                    Name = "Dashboard",
                    IsHeader = false,
                    IsModule = true,
                    IsComponent = false,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/home",
                    RouteLinkClass = "nav-link active",
                    Icon = "nav-icon fas fa-tachometer-alt",
                    Remark = "Navigation Item",
                    ParentId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                    DropdownIcon = null,
                    SerialNo = 2,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                    Name = "Business",
                    IsHeader = true,
                    IsModule = false,
                    IsComponent = false,
                    CssClass = "nav-header",
                    IsRouteLink = false,
                    RouteLink = "",
                    RouteLinkClass = "",
                    Icon = "",
                    Remark = "Header",
                    ParentId = null,
                    DropdownIcon = null,
                    SerialNo = 3,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    Name = "Security",
                    IsHeader = false,
                    IsModule = true,
                    IsComponent = false,
                    CssClass = "nav-item",
                    IsRouteLink = false,
                    RouteLink = "",
                    RouteLinkClass = "nav-link active",
                    Icon = "nav-icon fas fa-cog",
                    Remark = "Navigation Item",
                    ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                    DropdownIcon = "fas fa-angle-left right",
                    SerialNo = 4,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                    Name = "User Role",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/security/appuserrole",
                    RouteLinkClass = "nav-link",
                    Icon = "far fa-circle nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    DropdownIcon = null,
                    SerialNo = 5,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("904B0D8A-CC96-4930-BB1C-65838AF2B2EF"),
                    Name = "User Menu",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/security/appusermenu",
                    RouteLinkClass = "nav-link",
                    Icon = "far fa-circle nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    DropdownIcon = null,
                    SerialNo = 5,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("0E5E1B9E-840D-4F96-ABD1-FDB3EFB8A55D"),
                    Name = "User Role Menu",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/security/appuserrolemenu",
                    RouteLinkClass = "nav-link",
                    Icon = "far fa-circle nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    DropdownIcon = null,
                    SerialNo = 5,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                //new AppUserMenu
                //{
                //    Id = new Guid("73CC3330-54E9-4152-9ED4-F7C31748985E"),
                //    Name = "User Profile",
                //    IsHeader = false,
                //    IsModule = false,
                //    IsComponent = true,
                //    CssClass = "nav-item",
                //    IsRouteLink = true,
                //    RouteLink = "/business/security/appuserprofile",
                //    RouteLinkClass = "nav-link",
                //    Icon = "far fa-circle nav-icon",
                //    Remark = "Navigation Item",
                //    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                //    DropdownIcon = null,
                //    SerialNo = 5,
                //    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                //    CreatedDate = menuTime,
                //    UpdatedBy = null,
                //    UpdatedDate = menuTime,
                //    IsActive = true
                //},
                //new AppUserMenu
                //{
                //    Id = new Guid("AD0983EB-F60C-4329-8516-1650537A0567"),
                //    Name = "Application User",
                //    IsHeader = false,
                //    IsModule = false,
                //    IsComponent = true,
                //    CssClass = "nav-item",
                //    IsRouteLink = true,
                //    RouteLink = "/business/security/appuser",
                //    RouteLinkClass = "nav-link",
                //    Icon = "far fa-circle nav-icon",
                //    Remark = "Navigation Item",
                //    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                //    DropdownIcon = null,
                //    SerialNo = 5,
                //    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                //    CreatedDate = menuTime,
                //    UpdatedBy = null,
                //    UpdatedDate = menuTime,
                //    IsActive = true
                //},
                new AppUserMenu
                {
                    Id = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                    Name = "Settings",
                    IsHeader = true,
                    IsModule = false,
                    IsComponent = false,
                    CssClass = "nav-header",
                    IsRouteLink = false,
                    RouteLink = "",
                    RouteLinkClass = "",
                    Icon = "",
                    Remark = "Header",
                    ParentId = null,
                    DropdownIcon = null,
                    SerialNo = 6,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
                    Name = "App Settings",
                    IsHeader = false,
                    IsModule = true,
                    IsComponent = false,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/appsettings",
                    RouteLinkClass = "nav-link active",
                    Icon = "nav-icon fas fa-cog",
                    Remark = "Navigation Item",
                    ParentId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                    DropdownIcon = null,
                    SerialNo = 7,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                });

            // ------------------------------------------------------------------
            // Seed: AppUserRoleMenu
            // (fixed Ids instead of Guid.NewGuid, fixed timestamps)
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserRoleMenu>().HasData(
                new AppUserRoleMenu
                {
                    Id = new Guid("1D3BEE1C-6126-4113-8599-C64C7F1D47F6"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("F6F1BFA4-1AA9-45E8-876D-CF1C4D959C38"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("5BCA0329-727C-46C1-9C05-7F54292B0B17"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("87F5E973-B247-4FA9-8C48-60F3FD6F2021"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("698ECE21-FD28-44BA-95F1-1FFEF963C5B5"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("E7E675A8-F9AA-4DEF-B952-EEE1B18C708A"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("8DD17156-46B2-4199-AD17-B32785209468"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("1122EBC9-40B0-43D1-BD60-F3B0F7666EA3"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("DC00DC24-6D35-47EE-8CED-5F119721628E"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
                    IsView = true,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("BB58E8FA-60CC-4505-BF83-21495ED9F8E3"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("904B0D8A-CC96-4930-BB1C-65838AF2B2EF"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("90503E10-3FC1-4DC2-A39B-2F2A9AA198F1"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("228DE240-DB62-4E40-8EF0-367C0F71286D"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = false
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("EB3008BC-7EF6-42CF-A6AF-50E720E47B8E"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("9119FBDA-76E6-4098-9ADA-B0E8B027FC24"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = false
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("6EE5CFF9-9CEF-450D-85A7-B9B1FC3B1431"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("0E5E1B9E-840D-4F96-ABD1-FDB3EFB8A55D"),
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = false,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("B5AAF803-3BA5-4003-804A-C8BB8973A481"),
                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
                    AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                    IsView = null,
                    IsCreate = null,
                    IsUpdate = null,
                    IsDelete = null,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = roleMenuTime,
                    UpdatedBy = null,
                    UpdatedDate = roleMenuTime,
                    IsActive = true
                });
            //modelBuilder.Entity<AppUserRole>().HasData(
            //      new AppUserRole()
            //      {
            //          Id = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          RoleName = ConstantSupplier.ADMIN,
            //          Description = ConstantSupplier.ADMIN,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRole()
            //      {
            //          Id = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
            //          RoleName = ConstantSupplier.USER,
            //          Description = ConstantSupplier.USER,
            //          CreatedBy = "1B15CE5A-56B3-4EB9-8286-6E27F770B0DA",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<AppUserProfile>().HasData(
            //      new AppUserProfile()
            //      {
            //          Id = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
            //          FullName = "Sreemonta Bhowmik",
            //          Address = "Dubai",
            //          Email = "sbhowmikcse08@gmail.com",
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserProfile()
            //      {
            //          Id = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
            //          FullName = "Anannya Rohine",
            //          Address = "Dubai",
            //          Email = "rohine2008@gmail.com",
            //          AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<AppUser>().HasData(
            //      new AppUser()
            //      {
            //          Id = new Guid("5FD67AAF-183E-48F8-BB53-15B7628A3E0A"),
            //          AppUserProfileId = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
            //          UserName = "sree",
            //          Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
            //          SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
            //          RefreshToken = null,
            //          RefreshTokenExpiryTime = null,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUser()
            //      {
            //          Id = new Guid("A9E00F47-89EC-46A3-A5E8-31EBD52AC121"),
            //          AppUserProfileId = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
            //          UserName = "rohine",
            //          Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
            //          SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
            //          RefreshToken = null,
            //          RefreshTokenExpiryTime = null,
            //          CreatedBy = null,
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<AppUserMenu>().HasData(
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
            //          Name = "Home",
            //          IsHeader = true,
            //          IsModule = false,
            //          IsComponent = false,
            //          CssClass = "nav-header",
            //          IsRouteLink = false,
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 1,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
            //          Name = "Dashboard",
            //          IsHeader = false,
            //          IsModule = true,
            //          IsComponent = false,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/home",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-tachometer-alt",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
            //          DropdownIcon = null,
            //          SerialNo = 2,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
            //          Name = "Business",
            //          IsHeader = true,
            //          IsModule = false,
            //          IsComponent = false,
            //          CssClass = "nav-header",
            //          IsRouteLink = false,
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 3,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          Name = "Security",
            //          IsHeader = false,
            //          IsModule = true,
            //          IsComponent = false,
            //          CssClass = "nav-item",
            //          IsRouteLink = false,
            //          RouteLink = "",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-cog",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
            //          DropdownIcon = "fas fa-angle-left right",
            //          SerialNo = 4,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
            //          Name = "User Role",
            //          IsHeader = false,
            //          IsModule = false,
            //          IsComponent = true,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/security/appuserrole",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("15F1D22F-6DA5-4ED7-9518-C0B85F7014AD"),
            //          Name = "User Menu",
            //          IsHeader = false,
            //          IsModule = false,
            //          IsComponent = true,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/security/appusermenu",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("1943AA7C-B50A-484D-9CAD-28456A20C2CC"),
            //          Name = "User Role Menu",
            //          IsHeader = false,
            //          IsModule = false,
            //          IsComponent = true,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/security/appuserrolemenu",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("73CC3330-54E9-4152-9ED4-F7C31748985E"),
            //          Name = "User Profile",
            //          IsHeader = false,
            //          IsModule = false,
            //          IsComponent = true,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/security/appuserprofile",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("AD0983EB-F60C-4329-8516-1650537A0567"),
            //          Name = "Application User",
            //          IsHeader = false,
            //          IsModule = false,
            //          IsComponent = true,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/security/appuser",
            //          RouteLinkClass = "nav-link",
            //          Icon = "far fa-circle nav-icon",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          DropdownIcon = null,
            //          SerialNo = 5,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
            //          Name = "Settings",
            //          IsHeader = true,
            //          IsModule = false,
            //          IsComponent = false,
            //          CssClass = "nav-header",
            //          IsRouteLink = false,
            //          RouteLink = "",
            //          RouteLinkClass = "",
            //          Icon = "",
            //          Remark = "Header",
            //          ParentId = null,
            //          DropdownIcon = null,
            //          SerialNo = 6,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserMenu()
            //      {
            //          Id = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
            //          Name = "App Settings",
            //          IsHeader = false,
            //          IsModule = true,
            //          IsComponent = false,
            //          CssClass = "nav-item",
            //          IsRouteLink = true,
            //          RouteLink = "/business/appsettings",
            //          RouteLinkClass = "nav-link active",
            //          Icon = "nav-icon fas fa-cog",
            //          Remark = "Navigation Item",
            //          ParentId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
            //          DropdownIcon = null,
            //          SerialNo = 7,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

            //modelBuilder.Entity<AppUserRoleMenu>().HasData(
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
            //          IsView = null,
            //          IsCreate = null,
            //          IsUpdate = null,
            //          IsDelete = null,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
            //          IsView = true,
            //          IsCreate = true,
            //          IsUpdate = true,
            //          IsDelete = true,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
            //          IsView = null,
            //          IsCreate = null,
            //          IsUpdate = null,
            //          IsDelete = null,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
            //          IsView = null,
            //          IsCreate = null,
            //          IsUpdate = null,
            //          IsDelete = null,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
            //          IsView = true,
            //          IsCreate = true,
            //          IsUpdate = true,
            //          IsDelete = true,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
            //          IsView = null,
            //          IsCreate = null,
            //          IsUpdate = null,
            //          IsDelete = null,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
            //          AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
            //          IsView = true,
            //          IsCreate = true,
            //          IsUpdate = true,
            //          IsDelete = true,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
            //          AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
            //          IsView = null,
            //          IsCreate = null,
            //          IsUpdate = null,
            //          IsDelete = null,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      },
            //      new AppUserRoleMenu()
            //      {
            //          Id = Guid.NewGuid(),
            //          AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
            //          AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
            //          IsView = true,
            //          IsCreate = false,
            //          IsUpdate = false,
            //          IsDelete = false,
            //          CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
            //          CreatedDate = DateTime.UtcNow,
            //          UpdatedBy = null,
            //          UpdatedDate = DateTime.UtcNow,
            //          IsActive = true
            //      });

        }
    }
}
