//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SBERP.Security.Models.Base;
//using SBERP.Security.Helper;
//using StackExchange.Redis;
//using FluentEmail.Core.Models;
//using SSBERP.Security.Models.Base;

//namespace SBERP.Security.Persistence
//{
//    public class SecurityDBContext : DbContext
//    {
//        public SecurityDBContext()
//        {
//        }
//        public SecurityDBContext(DbContextOptions<SecurityDBContext> options) : base(options)
//        {

//        }
//        public virtual DbSet<AppUserRole>? AppUserRoles { get; set; }
//        public virtual DbSet<AppUserRoleLog>? AppUserRolesLog { get; set; }
//        public virtual DbSet<AppUserProfile>? AppUserProfiles { get; set; }
//        public virtual DbSet<AppUserProfileLog>? AppUserProfilesLog { get; set; }
//        public virtual DbSet<AppUser>? AppUsers { get; set; }
//        public virtual DbSet<AppUserLog>? AppUsersLog { get; set; }
//        public virtual DbSet<AppUserMenu>? AppUserMenus { get; set; }
//        public virtual DbSet<AppUserMenuLog>? AppUserMenusLog { get; set; }
//        public virtual DbSet<AppUserRoleMenu>? AppUserRoleMenus { get; set; }
//        public virtual DbSet<AppUserRoleMenuLog>? AppUserRoleMenusLog { get; set; }


//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // ------------------------------------------------------------------
//            // Primary keys
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRole>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserRoleLog>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserProfile>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserProfileLog>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUser>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserLog>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserMenu>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserMenuLog>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserRoleMenu>().HasKey(x => x.Id);
//            modelBuilder.Entity<AppUserRoleMenuLog>().HasKey(x => x.Id);

//            // ------------------------------------------------------------------
//            // AppUserRole
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRole>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");

//                entity.Property(x => x.RoleName).HasMaxLength(50);
//                entity.Property(x => x.Description).HasMaxLength(100);
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoles"));
//                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoles"));
//                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoles"));
//                entity.ToTable(table =>
//                {
//                    table.HasTrigger("TRG_InsertAppUserRoles");
//                    table.HasTrigger("TRG_UpdateAppUserRoles");
//                    table.HasTrigger("TRG_DeleteAppUserRoles");
//                });
//            });

//            // ------------------------------------------------------------------
//            // AppUserRoleLog
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRoleLog>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
//                entity.Property(x => x.RoleName).HasMaxLength(50);
//                entity.Property(x => x.Description).HasMaxLength(100);
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
//                entity.Property(x => x.Action).HasColumnName("Action");
//            });

//            // ------------------------------------------------------------------
//            // AppUserMenu
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserMenu>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");

//                entity.Property(x => x.Name).HasMaxLength(100);
//                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
//                entity.Property(x => x.IsModule).HasColumnName("IsModule");
//                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
//                entity.Property(x => x.CssClass).HasMaxLength(100);
//                entity.Property(x => x.IsRouteLink).HasColumnName("IsRouteLink");
//                entity.Property(x => x.RouteLink).HasMaxLength(255);
//                entity.Property(x => x.RouteLinkClass).HasMaxLength(200);
//                entity.Property(x => x.Icon).HasMaxLength(100);
//                entity.Property(x => x.Remark).HasMaxLength(255);
//                entity.Property(x => x.ParentId).HasColumnName("ParentId");
//                entity.Property(x => x.DropdownIcon).HasMaxLength(100);
//                entity.Property(x => x.SerialNo).HasColumnName("SerialNo");
//                entity.Property(x => x.Remark).HasMaxLength(255);
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserMenus"));
//                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserMenus"));
//                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserMenus"));
//                entity.ToTable(table =>
//                {
//                    table.HasTrigger("TRG_InsertAppUserMenus");
//                    table.HasTrigger("TRG_UpdateAppUserMenus");
//                    table.HasTrigger("TRG_DeleteAppUserMenus");
//                });
//            });

//            // ------------------------------------------------------------------
//            // AppUserMenuLog
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserMenuLog>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
//                entity.Property(x => x.Name).HasMaxLength(100);
//                entity.Property(x => x.IsHeader).HasColumnName("IsHeader");
//                entity.Property(x => x.IsModule).HasColumnName("IsModule");
//                entity.Property(x => x.IsComponent).HasColumnName("IsComponent");
//                entity.Property(x => x.CssClass).HasMaxLength(100);
//                entity.Property(x => x.IsRouteLink).HasColumnName("IsRouteLink");
//                entity.Property(x => x.RouteLink).HasMaxLength(255);
//                entity.Property(x => x.RouteLinkClass).HasMaxLength(200);
//                entity.Property(x => x.Icon).HasMaxLength(100);
//                entity.Property(x => x.Remark).HasMaxLength(255);
//                entity.Property(x => x.ParentId).HasColumnName("ParentId");
//                entity.Property(x => x.DropdownIcon).HasMaxLength(100);
//                entity.Property(x => x.SerialNo).HasColumnName("SerialNo");
//                entity.Property(x => x.Remark).HasMaxLength(255);
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
//                entity.Property(x => x.Action).HasColumnName("Action");
//            });

//            // ------------------------------------------------------------------
//            // AppUserProfile
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserProfile>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.FullName).HasMaxLength(200);
//                entity.Property(x => x.Address).HasMaxLength(200);
//                entity.Property(x => x.Email).HasMaxLength(200);
//                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.HasOne(x => x.AppUserRole)
//                    .WithMany(p => p.AppUserProfiles)
//                    .HasForeignKey(d => d.AppUserRoleId)
//                    .HasConstraintName("FK_AppUserProfiles_AppUserRole");
//                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserProfiles"));
//                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserProfiles"));
//                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserProfiles"));
//                entity.ToTable(table =>
//                {
//                    table.HasTrigger("TRG_InsertAppUserProfiles");
//                    table.HasTrigger("TRG_UpdateAppUserProfiles");
//                    table.HasTrigger("TRG_DeleteAppUserProfiles");
//                });
//            });

//            // ------------------------------------------------------------------
//            // AppUserProfileLog
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserProfileLog>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
//                entity.Property(x => x.FullName).HasMaxLength(200);
//                entity.Property(x => x.Address).HasMaxLength(200);
//                entity.Property(x => x.Email).HasMaxLength(200);
//                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
//                entity.Property(x => x.Action).HasColumnName("Action");
//            });

//            // ------------------------------------------------------------------
//            // AppUserRoleMenu
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRoleMenu>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
//                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
//                entity.Property(x => x.IsView).HasColumnName("IsView");
//                entity.Property(x => x.IsCreate).HasColumnName("IsCreate");
//                entity.Property(x => x.IsUpdate).HasColumnName("IsUpdate");
//                entity.Property(x => x.IsDelete).HasColumnName("IsDelete");
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");

//                entity.HasOne(x => x.AppUserRole)
//                    .WithMany(p => p.AppUserRoleMenus)
//                    .HasForeignKey(d => d.AppUserRoleId)
//                    .HasConstraintName("FK_AppUserRoleMenus_AppUserRole");

//                entity.HasOne(x => x.AppUserMenu)
//                    .WithMany(x => x.AppUserRoleMenus)
//                    .HasForeignKey(x => x.AppUserMenuId)
//                    .HasConstraintName("FK_AppUserRoleMenus_AppUserMenu");

//                //entity.ToTable(x => x.HasTrigger("TRG_InsertAppUserRoleMenus"));
//                //entity.ToTable(x => x.HasTrigger("TRG_UpdateAppUserRoleMenus"));
//                //entity.ToTable(x => x.HasTrigger("TRG_DeleteAppUserRoleMenus"));
//                entity.ToTable(table =>
//                {
//                    table.HasTrigger("TRG_InsertAppUserRoleMenus");
//                    table.HasTrigger("TRG_UpdateAppUserRoleMenus");
//                    table.HasTrigger("TRG_DeleteAppUserRoleMenus");
//                });
//            });

//            // ------------------------------------------------------------------
//            // AppUserRoleMenuLog
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRoleMenuLog>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserRoleMenuId).HasColumnName("AppUserRoleMenuId");
//                entity.Property(x => x.AppUserRoleId).HasColumnName("AppUserRoleId");
//                entity.Property(x => x.AppUserMenuId).HasColumnName("AppUserMenuId");
//                entity.Property(x => x.IsView).HasColumnName("IsView");
//                entity.Property(x => x.IsCreate).HasColumnName("IsCreate");
//                entity.Property(x => x.IsUpdate).HasColumnName("IsUpdate");
//                entity.Property(x => x.IsDelete).HasColumnName("IsDelete");
//                entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
//                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
//                entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
//                entity.Property(x => x.UpdatedDate).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
//                entity.Property(x => x.Action).HasColumnName("Action");
//            });

//            // ------------------------------------------------------------------
//            // AppUser
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUser>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
//                entity.Property(x => x.UserName).HasMaxLength(100).IsRequired();
//                entity.Property(x => x.Password).HasMaxLength(255);
//                entity.Property(x => x.SaltKey).HasMaxLength(255);
//                entity.Property(x => x.RefreshToken).HasMaxLength(255);
//                entity.Property(x => x.RefreshTokenExpiryTime).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");

//                entity.HasOne(x => x.AppUserProfile)
//                    .WithOne(p => p.AppUser)
//                    .HasForeignKey<AppUser>(x => x.AppUserProfileId)
//                    .HasConstraintName("FK_AppUser_AppUserProfiles");

//                // Add unique index on UserName
//                entity.HasIndex(x => x.UserName).IsUnique().HasDatabaseName("UX_AppUsers_UserName");  // <-- NEW LINE: Enforces unique UserName
//                entity.ToTable(table =>
//                {
//                    table.HasTrigger("TRG_InsertAppUsers");
//                    table.HasTrigger("TRG_UpdateAppUsers");
//                    table.HasTrigger("TRG_DeleteAppUsers");
//                });
//            });

//            // ------------------------------------------------------------------
//            // AppUserLog
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserLog>(entity =>
//            {
//                entity.Property(x => x.Id).HasColumnName("Id");
//                entity.Property(x => x.AppUserId).HasColumnName("AppUserId");
//                entity.Property(x => x.AppUserProfileId).HasColumnName("AppUserProfileId");
//                entity.Property(x => x.UserName).HasMaxLength(100);
//                entity.Property(x => x.Password).HasMaxLength(255);
//                entity.Property(x => x.SaltKey).HasMaxLength(255);
//                entity.Property(x => x.RefreshToken).HasMaxLength(255);
//                entity.Property(x => x.RefreshTokenExpiryTime).HasColumnType("datetime");
//                entity.Property(x => x.IsActive).HasColumnName("IsActive");
//                entity.Property(x => x.PerformedUser).HasColumnName("PerformedUser");
//                entity.Property(x => x.Action).HasColumnName("Action");
//            });

//            // ==================================================================
//            // SEED DATA  (all deterministic – no DateTime.UtcNow / Guid.NewGuid)
//            // ==================================================================
//            var roleTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
//            var profileTime = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);
//            var userTime = new DateTime(2024, 1, 1, 10, 10, 0, DateTimeKind.Utc);
//            var menuTime = new DateTime(2024, 1, 1, 10, 15, 0, DateTimeKind.Utc);
//            var roleMenuTime = new DateTime(2024, 1, 1, 10, 20, 0, DateTimeKind.Utc);

//            // ------------------------------------------------------------------
//            // Seed: AppUserRole
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRole>().HasData(
//                new AppUserRole
//                {
//                    Id = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    RoleName = ConstantSupplier.ADMIN,
//                    Description = ConstantSupplier.ADMIN,
//                    CreatedBy = null,
//                    CreatedDate = roleTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleTime,
//                    IsActive = true
//                },
//                new AppUserRole
//                {
//                    Id = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    RoleName = ConstantSupplier.USER,
//                    Description = ConstantSupplier.USER,
//                    CreatedBy = "1B15CE5A-56B3-4EB9-8286-6E27F770B0DA",
//                    CreatedDate = roleTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleTime,
//                    IsActive = true
//                });


//            // ------------------------------------------------------------------
//            // Seed: AppUserProfile
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserProfile>().HasData(
//                new AppUserProfile
//                {
//                    Id = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
//                    FullName = "Sreemonta Bhowmik",
//                    Address = "Dubai",
//                    Email = "sbhowmikcse08@gmail.com",
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    CreatedBy = null,
//                    CreatedDate = profileTime,
//                    UpdatedBy = null,
//                    UpdatedDate = profileTime,
//                    IsActive = true
//                },
//                new AppUserProfile
//                {
//                    Id = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
//                    FullName = "Anannya Rohine",
//                    Address = "Dubai",
//                    Email = "rohine2008@gmail.com",
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = profileTime,
//                    UpdatedBy = null,
//                    UpdatedDate = profileTime,
//                    IsActive = true
//                });

//            // ------------------------------------------------------------------
//            // Seed: AppUser
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUser>().HasData(
//                new AppUser
//                {
//                    Id = new Guid("5FD67AAF-183E-48F8-BB53-15B7628A3E0A"),
//                    AppUserProfileId = new Guid("C047D662-9F0E-4358-B323-15EC3081312C"),
//                    UserName = "sree",
//                    Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
//                    SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
//                    RefreshToken = null,
//                    RefreshTokenExpiryTime = null,
//                    CreatedBy = null,
//                    CreatedDate = userTime,
//                    UpdatedBy = null,
//                    UpdatedDate = userTime,
//                    IsActive = true
//                },
//                new AppUser
//                {
//                    Id = new Guid("A9E00F47-89EC-46A3-A5E8-31EBD52AC121"),
//                    AppUserProfileId = new Guid("EFEDC118-3459-4C2E-9158-AD69196A59E0"),
//                    UserName = "rohine",
//                    Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
//                    SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
//                    RefreshToken = null,
//                    RefreshTokenExpiryTime = null,
//                    CreatedBy = null,
//                    CreatedDate = userTime,
//                    UpdatedBy = null,
//                    UpdatedDate = userTime,
//                    IsActive = true
//                });

//            // ------------------------------------------------------------------
//            // Seed: AppUserMenu
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserMenu>().HasData(
//                new AppUserMenu
//                {
//                    Id = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
//                    Name = "Home",
//                    IsHeader = true,
//                    IsModule = false,
//                    IsComponent = false,
//                    CssClass = "nav-header",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "",
//                    Icon = "",
//                    Remark = "Header",
//                    ParentId = null,
//                    DropdownIcon = null,
//                    SerialNo = 1,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
//                    Name = "Dashboard",
//                    IsHeader = false,
//                    IsModule = true,
//                    IsComponent = false,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/home",
//                    RouteLinkClass = "nav-link active",
//                    Icon = "nav-icon fas fa-tachometer-alt",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
//                    DropdownIcon = null,
//                    SerialNo = 2,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
//                    Name = "Business",
//                    IsHeader = true,
//                    IsModule = false,
//                    IsComponent = false,
//                    CssClass = "nav-header",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "",
//                    Icon = "",
//                    Remark = "Header",
//                    ParentId = null,
//                    DropdownIcon = null,
//                    SerialNo = 3,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    Name = "Security",
//                    IsHeader = false,
//                    IsModule = true,
//                    IsComponent = false,
//                    CssClass = "nav-item",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "nav-link active",
//                    Icon = "fas fa-shield-alt nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
//                    DropdownIcon = "fas fa-angle-left right",
//                    SerialNo = 4,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
//                    Name = "User Role",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/security/appuserrole",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-briefcase nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    DropdownIcon = null,
//                    SerialNo = 5,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("904B0D8A-CC96-4930-BB1C-65838AF2B2EF"),
//                    Name = "User Menu",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/security/appusermenu",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-user-cog nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    DropdownIcon = null,
//                    SerialNo = 6,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("0E5E1B9E-840D-4F96-ABD1-FDB3EFB8A55D"),
//                    Name = "User Role Menu",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/security/appuserrolemenu",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-user-lock nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    DropdownIcon = null,
//                    SerialNo = 7,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("96695CA5-6010-474C-BFD1-F9746FABD81A"),
//                    Name = "User Profile",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/security/appuserprofile",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-user-circle nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    DropdownIcon = null,
//                    SerialNo = 8,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
//                    Name = "Settings",
//                    IsHeader = true,
//                    IsModule = false,
//                    IsComponent = false,
//                    CssClass = "nav-header",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "",
//                    Icon = "",
//                    Remark = "Header",
//                    ParentId = null,
//                    DropdownIcon = null,
//                    SerialNo = 9,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
//                    Name = "App Settings",
//                    IsHeader = false,
//                    IsModule = true,
//                    IsComponent = false,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/appsettings",
//                    RouteLinkClass = "nav-link active",
//                    Icon = "nav-icon fas fa-cog",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
//                    DropdownIcon = null,
//                    SerialNo = 10,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    Name = "Human Resources",
//                    IsHeader = false,
//                    IsModule = true,
//                    IsComponent = false,
//                    CssClass = "nav-item",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "nav-link active",
//                    Icon = "fas fa-users nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
//                    DropdownIcon = "fas fa-angle-left right",
//                    SerialNo = 11,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("A0116982-F0B2-4C4E-AB05-0A59B0B2A0B8"),
//                    Name = "Department",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/department",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-sitemap nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    DropdownIcon = null,
//                    SerialNo = 12,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("89614EF6-4931-458F-8611-DE1EA89A2C2D"),
//                    Name = "Designation",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/designation",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-briefcase nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    DropdownIcon = null,
//                    SerialNo = 13,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("171253A9-0BEB-4722-8FC4-A4C18895B980"),
//                    Name = "Employee",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/employee",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-user-tie nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    DropdownIcon = null,
//                    SerialNo = 14,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("05FBCA7E-8D98-4C4B-9529-4DFB29DB65FD"),
//                    Name = "HR Settings",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/settings",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-cogs nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    DropdownIcon = null,
//                    SerialNo = 15,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                // === Organization Setup (Module 0) ===
//                new AppUserMenu
//                {
//                    Id = new Guid("CF7F9B36-4CCA-46B0-B8CC-E7AD078C7EB9"),
//                    Name = "Organization Setup",
//                    IsHeader = false,
//                    IsModule = true,
//                    IsComponent = false,
//                    CssClass = "nav-item",
//                    IsRouteLink = false,
//                    RouteLink = "",
//                    RouteLinkClass = "nav-link active",
//                    Icon = "fas fa-building nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),   // "Business" header
//                    DropdownIcon = "fas fa-angle-left right",
//                    SerialNo = 16,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("082295E5-CD29-4693-8CF9-8082D4F8E791"),
//                    Name = "Company",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/company",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-industry nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("CF7F9B36-4CCA-46B0-B8CC-E7AD078C7EB9"),   // "Organization Setup"
//                    DropdownIcon = null,
//                    SerialNo = 17,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                },
//                new AppUserMenu
//                {
//                    Id = new Guid("88664718-2161-44CE-B940-39C952E7F56C"),
//                    Name = "Branch",
//                    IsHeader = false,
//                    IsModule = false,
//                    IsComponent = true,
//                    CssClass = "nav-item",
//                    IsRouteLink = true,
//                    RouteLink = "/business/humanresources/branch",
//                    RouteLinkClass = "nav-link",
//                    Icon = "fas fa-code-branch nav-icon",
//                    Remark = "Navigation Item",
//                    ParentId = new Guid("CF7F9B36-4CCA-46B0-B8CC-E7AD078C7EB9"),   // "Organization Setup"
//                    DropdownIcon = null,
//                    SerialNo = 18,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = menuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = menuTime,
//                    IsActive = true
//                });

//            // ------------------------------------------------------------------
//            // Seed: AppUserRoleMenu
//            // (fixed Ids instead of Guid.NewGuid, fixed timestamps)
//            // ------------------------------------------------------------------
//            modelBuilder.Entity<AppUserRoleMenu>().HasData(
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("1D3BEE1C-6126-4113-8599-C64C7F1D47F6"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("F6F1BFA4-1AA9-45E8-876D-CF1C4D959C38"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("5BCA0329-727C-46C1-9C05-7F54292B0B17"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("87F5E973-B247-4FA9-8C48-60F3FD6F2021"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("698ECE21-FD28-44BA-95F1-1FFEF963C5B5"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("E7E675A8-F9AA-4DEF-B952-EEE1B18C708A"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("8DD17156-46B2-4199-AD17-B32785209468"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("1122EBC9-40B0-43D1-BD60-F3B0F7666EA3"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("DB0085B7-695D-4751-A190-6C52E3BB44F1"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("DC00DC24-6D35-47EE-8CED-5F119721628E"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B"),
//                    IsView = true,
//                    IsCreate = false,
//                    IsUpdate = false,
//                    IsDelete = false,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("BB58E8FA-60CC-4505-BF83-21495ED9F8E3"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("904B0D8A-CC96-4930-BB1C-65838AF2B2EF"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("90503E10-3FC1-4DC2-A39B-2F2A9AA198F1"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("60AADC18-6B91-4CEE-ACE7-97700B685C98"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("228DE240-DB62-4E40-8EF0-367C0F71286D"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = false
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("EB3008BC-7EF6-42CF-A6AF-50E720E47B8E"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("9119FBDA-76E6-4098-9ADA-B0E8B027FC24"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("52F916CC-6C4D-4B4F-B884-4E89F1489B8D"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = false
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("6EE5CFF9-9CEF-450D-85A7-B9B1FC3B1431"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("0E5E1B9E-840D-4F96-ABD1-FDB3EFB8A55D"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = false,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("B5AAF803-3BA5-4003-804A-C8BB8973A481"),
//                    AppUserRoleId = new Guid("10A9E9E7-CB24-4816-9B94-9DB275A40EDD"),
//                    AppUserMenuId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("99C2D21A-2197-4334-A86C-AE9416DBFB67"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("96695CA5-6010-474C-BFD1-F9746FABD81A"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("CB2F5FEB-0C2E-4A7F-9483-0E61FC83B140"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
//                    IsView = false,
//                    IsCreate = false,
//                    IsUpdate = false,
//                    IsDelete = false,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("BBBACB3F-E5B8-4AD7-A88F-894AF652A62F"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("A0116982-F0B2-4C4E-AB05-0A59B0B2A0B8"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("C90C4EB9-A475-4C07-AC6C-EA4C22B93EB6"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("89614EF6-4931-458F-8611-DE1EA89A2C2D"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("5B40C437-8827-413B-92A7-239B42186A05"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("171253A9-0BEB-4722-8FC4-A4C18895B980"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("0BA66610-AA06-4FCB-A350-B9F9018E5FD9"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("05FBCA7E-8D98-4C4B-9529-4DFB29DB65FD"),
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                // === Organization Setup (Module 0) — full CRUD for the seeded
//                // super-admin role, same as every other module row above ===
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("94944577-973E-423C-8AAC-0712C1C33B58"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("CF7F9B36-4CCA-46B0-B8CC-E7AD078C7EB9"),   // Organization Setup (header row)
//                    IsView = null,
//                    IsCreate = null,
//                    IsUpdate = null,
//                    IsDelete = null,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("00865190-7ECB-4C61-B2BF-CE4DCDF83092"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("082295E5-CD29-4693-8CF9-8082D4F8E791"),   // Company
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                },
//                new AppUserRoleMenu
//                {
//                    Id = new Guid("B136043F-372B-49D5-9A26-AD6331B89724"),
//                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
//                    AppUserMenuId = new Guid("88664718-2161-44CE-B940-39C952E7F56C"),   // Branch
//                    IsView = true,
//                    IsCreate = true,
//                    IsUpdate = true,
//                    IsDelete = true,
//                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
//                    CreatedDate = roleMenuTime,
//                    UpdatedBy = null,
//                    UpdatedDate = roleMenuTime,
//                    IsActive = true
//                });
//        }
//    }
//}

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
        public virtual DbSet<AppUserRefreshToken>? AppUserRefreshTokens { get; set; }
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
                entity.Property(x => x.IsSubModule).HasColumnName("IsSubModule");
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
                entity.Property(x => x.IsSubModule).HasColumnName("IsSubModule");
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
                entity.Property(x => x.JobTitle).HasMaxLength(100);
                entity.Property(x => x.ProfilePhotoId).HasColumnName("ProfilePhotoId");
                entity.Property(x => x.ProfilePhotoLocation).HasMaxLength(500);
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
                entity.Property(x => x.JobTitle).HasMaxLength(100);
                entity.Property(x => x.ProfilePhotoId).HasColumnName("ProfilePhotoId");
                entity.Property(x => x.ProfilePhotoLocation).HasMaxLength(500);
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
                entity.Property(x => x.UserName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Password).HasMaxLength(255);
                entity.Property(x => x.SaltKey).HasMaxLength(255);
                entity.Property(x => x.FailedLoginAttempts).HasDefaultValue(0);
                entity.Property(x => x.LockoutEnd).HasColumnType("datetime");
                entity.Property(x => x.IsActive).HasColumnName("IsActive");

                entity.HasOne(x => x.AppUserProfile)
                    .WithOne(p => p.AppUser)
                    .HasForeignKey<AppUser>(x => x.AppUserProfileId)
                    .HasConstraintName("FK_AppUser_AppUserProfiles");

                // Add unique index on UserName
                entity.HasIndex(x => x.UserName).IsUnique().HasDatabaseName("UX_AppUsers_UserName");  // <-- NEW LINE: Enforces unique UserName
                entity.ToTable(table =>
                {
                    table.HasTrigger("TRG_InsertAppUsers");
                    table.HasTrigger("TRG_UpdateAppUsers");
                    table.HasTrigger("TRG_DeleteAppUsers");
                });
            });

            // ------------------------------------------------------------------
            // AppUserRefreshToken  (hashed, rotated refresh tokens; one family per login)
            // ------------------------------------------------------------------
            modelBuilder.Entity<AppUserRefreshToken>(entity =>
            {
                entity.ToTable("AppUserRefreshTokens");
                entity.Property(x => x.TokenHash).HasMaxLength(64).IsUnicode(false).IsRequired();
                entity.Property(x => x.ReplacedByTokenHash).HasMaxLength(64).IsUnicode(false);
                entity.Property(x => x.ExpiresAt).HasColumnType("datetime");
                entity.Property(x => x.SessionExpiresAt).HasColumnType("datetime");
                entity.Property(x => x.CreatedDate).HasColumnType("datetime");
                entity.Property(x => x.RevokedDate).HasColumnType("datetime");
                entity.Property(x => x.CreatedByIp).HasMaxLength(64);
                entity.Property(x => x.RevokedByIp).HasMaxLength(64);
                entity.Property(x => x.UserAgent).HasMaxLength(512);
                entity.Property(x => x.RevokedReason).HasMaxLength(50);

                entity.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("UX_AppUserRefreshTokens_TokenHash");
                entity.HasIndex(x => x.FamilyId).HasDatabaseName("IX_AppUserRefreshTokens_FamilyId");
                entity.HasIndex(x => new { x.AppUserId, x.RevokedDate }).HasDatabaseName("IX_AppUserRefreshTokens_AppUserId_RevokedDate");

                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.AppUserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AppUserRefreshTokens_AppUsers");
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
                    JobTitle = "Chief Executive Officer",
                    ProfilePhotoId = null,          // no photo yet: the default avatar is shown
                    ProfilePhotoLocation = null,
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
                    JobTitle = "Software Engineer",
                    ProfilePhotoId = null,
                    ProfilePhotoLocation = null,
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
                    RouteLinkClass = "nav-link",
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
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-shield-alt nav-icon",
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
                    Icon = "fas fa-briefcase nav-icon",
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
                    Icon = "fas fa-user-cog nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
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
                    Id = new Guid("0E5E1B9E-840D-4F96-ABD1-FDB3EFB8A55D"),
                    Name = "User Role Menu",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/security/appuserrolemenu",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-user-lock nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    DropdownIcon = null,
                    SerialNo = 7,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("96695CA5-6010-474C-BFD1-F9746FABD81A"),
                    Name = "User Profile",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/security/appuserprofile",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-user-circle nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("C15215C8-32CA-4182-9510-B57419708A80"),
                    DropdownIcon = null,
                    SerialNo = 8,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
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
                    SerialNo = 9,
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
                    RouteLinkClass = "nav-link",
                    Icon = "nav-icon fas fa-cog",
                    Remark = "Navigation Item",
                    ParentId = new Guid("52D7E13B-EF24-4F17-937B-D6E8005A6658"),
                    DropdownIcon = null,
                    SerialNo = 10,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
                    Name = "Human Resources",
                    IsHeader = false,
                    IsModule = true,
                    IsComponent = false,
                    CssClass = "nav-item",
                    IsRouteLink = false,
                    RouteLink = "",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-users nav-icon",
                    Remark = "Navigation Item",
                    ParentId = new Guid("F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C"),
                    DropdownIcon = "fas fa-angle-left right",
                    SerialNo = 11,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("A0116982-F0B2-4C4E-AB05-0A59B0B2A0B8"),
                    Name = "Department",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/department",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-sitemap nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 15,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("89614EF6-4931-458F-8611-DE1EA89A2C2D"),
                    Name = "Designation",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/designation",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-briefcase nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 16,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("171253A9-0BEB-4722-8FC4-A4C18895B980"),
                    Name = "Employee",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/employee",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-user-tie nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 17,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("05FBCA7E-8D98-4C4B-9529-4DFB29DB65FD"),
                    Name = "HR Settings",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/settings",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-cogs nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 18,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                // === Company / Branch (Module 0) — Organization Setup sub-module of
                // "Human Resources" (see SecurityDBContext.HRMenuSeed.cs). ===
                new AppUserMenu
                {
                    Id = new Guid("082295E5-CD29-4693-8CF9-8082D4F8E791"),
                    Name = "Company",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/company",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-industry nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 13,
                    CreatedBy = "C047D662-9F0E-4358-B323-15EC3081312C",
                    CreatedDate = menuTime,
                    UpdatedBy = null,
                    UpdatedDate = menuTime,
                    IsActive = true
                },
                new AppUserMenu
                {
                    Id = new Guid("88664718-2161-44CE-B940-39C952E7F56C"),
                    Name = "Branch",
                    IsHeader = false,
                    IsModule = false,
                    IsComponent = true,
                    CssClass = "nav-item",
                    IsRouteLink = true,
                    RouteLink = "/business/humanresources/corehr/branch",
                    RouteLinkClass = "nav-link",
                    Icon = "fas fa-code-branch nav-icon",
                    Remark = "Navigation Item",
                    ParentId = CoreHRMenuId,   // HR sub-module (HRMenuSeed.cs)
                    DropdownIcon = null,
                    SerialNo = 14,
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
                },
                new AppUserRoleMenu
                {
                    Id = new Guid("99C2D21A-2197-4334-A86C-AE9416DBFB67"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("96695CA5-6010-474C-BFD1-F9746FABD81A"),
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
                    Id = new Guid("CB2F5FEB-0C2E-4A7F-9483-0E61FC83B140"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("2AC99AE0-C481-4A9C-A129-4CB3024B642E"),
                    IsView = false,
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
                    Id = new Guid("BBBACB3F-E5B8-4AD7-A88F-894AF652A62F"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("A0116982-F0B2-4C4E-AB05-0A59B0B2A0B8"),
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
                    Id = new Guid("C90C4EB9-A475-4C07-AC6C-EA4C22B93EB6"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("89614EF6-4931-458F-8611-DE1EA89A2C2D"),
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
                    Id = new Guid("5B40C437-8827-413B-92A7-239B42186A05"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("171253A9-0BEB-4722-8FC4-A4C18895B980"),
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
                    Id = new Guid("0BA66610-AA06-4FCB-A350-B9F9018E5FD9"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("05FBCA7E-8D98-4C4B-9529-4DFB29DB65FD"),
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
                // === Company / Branch (Module 0) — full CRUD for the seeded
                // super-admin role, now siblings of Employee/Department under
                // "Human Resources" rather than their own module header. ===
                new AppUserRoleMenu
                {
                    Id = new Guid("00865190-7ECB-4C61-B2BF-CE4DCDF83092"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("082295E5-CD29-4693-8CF9-8082D4F8E791"),   // Company
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
                    Id = new Guid("B136043F-372B-49D5-9A26-AD6331B89724"),
                    AppUserRoleId = new Guid("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA"),
                    AppUserMenuId = new Guid("88664718-2161-44CE-B940-39C952E7F56C"),   // Branch
                    IsView = true,
                    IsCreate = true,
                    IsUpdate = true,
                    IsDelete = true,
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

            // HR sub-module navigation (Org Setup → Separation) — see SecurityDBContext.HRMenuSeed.cs
            SeedHRSubModuleMenus(modelBuilder);

            // Onboarding provisioning: notifications, e-mail outbox, profile -> employee link
            ConfigureOnboardingProvisioning(modelBuilder);
        }

        #region HRMenuSeed (was SecurityDBContext.HRMenuSeed.cs)
        // Navigation for the HumanResources sub-modules (Module 0 Organization Setup
        // through Module 8 Separation).
        // Menu tree (any depth is supported by GetChildMenus and the sidebar):
        //   Header (IsHeader)  "Business"
        //     Module (IsModule)  "Human Resources"           – one per microservice
        //       Sub-module (IsSubModule)  "Recruitment" …    – optional grouping level
        //         Component (IsComponent)  "Requisition" … – a routed screen
        // A module may also hold components directly (e.g. "Security") or be a routed
        // leaf itself (e.g. "Dashboard", "App Settings").
        // RouteLink matches the Angular lazy routes (commonService.getMenuPermission(router.url)
        // resolves CRUD flags by URL). The Admin role gets full rights; the User role gets view
        // rights on the self-service screens (leave applications, goals, reviews) plus the
        // ancestor rows needed to reach them.
        private static readonly Guid HumanResourcesMenuId = new("2AC99AE0-C481-4A9C-A129-4CB3024B642E");
        private static readonly Guid AdminRoleId      = new("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA");
        private static readonly Guid UserRoleId       = new("10A9E9E7-CB24-4816-9B94-9DB275A40EDD");
        private const string SeedActor                = "C047D662-9F0E-4358-B323-15EC3081312C";

        private static Guid MenuId(int n) => new($"5E1D0000-0000-4000-8000-{n:D12}");
        private static Guid RoleMenuId(int n) => new($"5E1D1000-0000-4000-8000-{n:D12}");

        // Core HR holds the original Core HR screens seeded in SecurityDBContext.cs (Company, Branch,
        // Department, Designation, Employee, HR Settings — /business/humanresources/corehr/*).
        // Ids 1..31 come from the loop below in declaration order; 100+ are fixed additions.
        private static readonly Guid CoreHRMenuId = MenuId(100);

        private void SeedHRSubModuleMenus(ModelBuilder modelBuilder)
        {
            var seedTime = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc);

            // (sub-module, icon, SerialNo, first child SerialNo, children: (name, route, icon, userCanView))
            // SerialNo follows the depth-first order of the Human Resources tree (HR = 11):
            // Core HR 12 (Company 13, Branch 14, Department 15, Designation 16, Employee 17, HR Settings 18),
            // Organization Setup 19 (20-23), Recruitment 24 …
            var tree = new (string Module, string Icon, int Serial, int FirstItemSerial, (string Name, string Route, string Icon, bool UserView)[] Items)[]
            {
                ("Organization Setup", "fas fa-building nav-icon", 19, 20, new[]
                {
                    ("Position",         "/business/humanresources/orgsetup/position",         "fas fa-chair nav-icon",           false),
                    ("Cost Center",      "/business/humanresources/orgsetup/cost-center",      "fas fa-coins nav-icon",           false),
                    ("Job Grade",        "/business/humanresources/orgsetup/job-grade",        "fas fa-layer-group nav-icon",     false),
                    ("Holiday Calendar", "/business/humanresources/orgsetup/holiday-calendar", "fas fa-calendar-alt nav-icon",    false),
                }),
                ("Recruitment", "fas fa-user-plus nav-icon", 24, 25, new[]
                {
                    ("Requisition",        "/business/humanresources/recruitment/requisition", "fas fa-file-signature nav-icon", false),
                    ("Job Posting",        "/business/humanresources/recruitment/job-posting", "fas fa-bullhorn nav-icon",       false),
                    ("Applicant Pipeline", "/business/humanresources/recruitment/pipeline",    "fas fa-stream nav-icon",         false),
                }),
                ("Onboarding", "fas fa-handshake nav-icon", 28, 29, new[]
                {
                    ("Onboarding", "/business/humanresources/onboarding/list", "fas fa-clipboard-check nav-icon", false),
                }),
                ("Time & Attendance", "fas fa-clock nav-icon", 30, 31, new[]
                {
                    ("Shift",              "/business/humanresources/attendance/shift",    "fas fa-business-time nav-icon", false),
                    ("Overtime",           "/business/humanresources/attendance/overtime", "fas fa-hourglass-half nav-icon", false),
                    ("Attendance Summary", "/business/humanresources/attendance/summary",  "fas fa-calendar-check nav-icon", false),
                }),
                ("Leave", "fas fa-plane-departure nav-icon", 34, 35, new[]
                {
                    ("Leave Application", "/business/humanresources/leave/applications", "fas fa-paper-plane nav-icon", true),
                    ("Leave Type",        "/business/humanresources/leave/types",        "fas fa-tags nav-icon",        false),
                    ("Leave Policy",      "/business/humanresources/leave/policies",     "fas fa-gavel nav-icon",       false),
                }),
                ("Payroll", "fas fa-money-check-alt nav-icon", 38, 39, new[]
                {
                    ("Payroll Run",      "/business/humanresources/payroll/runs",              "fas fa-play-circle nav-icon",   false),
                    ("Salary Structure", "/business/humanresources/payroll/salary-structures", "fas fa-sitemap nav-icon",       false),
                    ("Pay Component",    "/business/humanresources/payroll/components",        "fas fa-puzzle-piece nav-icon",  false),
                    ("Loan / Advance",   "/business/humanresources/payroll/loans",             "fas fa-hand-holding-usd nav-icon", false),
                }),
                ("Performance", "fas fa-chart-line nav-icon", 43, 44, new[]
                {
                    ("Appraisal Cycle", "/business/humanresources/performance/cycles",       "fas fa-sync-alt nav-icon",  false),
                    ("Goal",            "/business/humanresources/performance/goals",        "fas fa-bullseye nav-icon",  true),
                    ("Review",          "/business/humanresources/performance/reviews",      "fas fa-star-half-alt nav-icon", true),
                    ("Competency",      "/business/humanresources/performance/competencies", "fas fa-brain nav-icon",     false),
                }),
                ("Separation", "fas fa-sign-out-alt nav-icon", 48, 49, new[]
                {
                    ("Separation", "/business/humanresources/separation", "fas fa-door-open nav-icon", false),
                }),
            };

            var menus = new List<AppUserMenu>();
            var roleMenus = new List<AppUserRoleMenu>();
            int n = 0, rm = 0;

            AppUserRoleMenu Grant(Guid id, Guid role, Guid menu, bool view, bool write) => new()
            {
                Id = id, AppUserRoleId = role, AppUserMenuId = menu,
                IsView = view, IsCreate = write, IsUpdate = write, IsDelete = write,
                CreatedBy = SeedActor, CreatedDate = seedTime, UpdatedDate = seedTime, IsActive = true
            };

            AppUserMenu SubModule(Guid id, string name, string icon, int serial) => new()
            {
                Id = id, Name = name, IsHeader = false, IsModule = false, IsSubModule = true, IsComponent = false,
                CssClass = "nav-item", IsRouteLink = false, RouteLink = "", RouteLinkClass = "nav-link",
                Icon = icon, Remark = "Navigation Item", ParentId = HumanResourcesMenuId,
                DropdownIcon = "fas fa-angle-left right", SerialNo = serial,
                CreatedBy = SeedActor, CreatedDate = seedTime, UpdatedDate = seedTime, IsActive = true
            };

            foreach (var (module, icon, serial, firstItemSerial, items) in tree)
            {
                var moduleId = MenuId(++n);
                menus.Add(SubModule(moduleId, module, icon, serial));
                // grouping rows carry no CRUD flags (same as the "Human Resources" module row)
                roleMenus.Add(Grant(RoleMenuId(++rm), AdminRoleId, moduleId, false, false));
                if (items.Any(i => i.UserView)) roleMenus.Add(Grant(RoleMenuId(++rm), UserRoleId, moduleId, false, false));

                int itemSerial = firstItemSerial;
                foreach (var (name, route, itemIcon, userView) in items)
                {
                    var itemId = MenuId(++n);
                    menus.Add(new AppUserMenu
                    {
                        Id = itemId, Name = name, IsHeader = false, IsModule = false, IsSubModule = false, IsComponent = true,
                        CssClass = "nav-item", IsRouteLink = true, RouteLink = route, RouteLinkClass = "nav-link",
                        Icon = itemIcon, Remark = "Navigation Item", ParentId = moduleId, DropdownIcon = null,
                        SerialNo = itemSerial++, CreatedBy = SeedActor, CreatedDate = seedTime, UpdatedDate = seedTime, IsActive = true
                    });
                    roleMenus.Add(Grant(RoleMenuId(++rm), AdminRoleId, itemId, true, true));
                    if (userView) roleMenus.Add(Grant(RoleMenuId(++rm), UserRoleId, itemId, true, false));
                }
            }

            // Core HR: groups the 6 Core HR screens (seeded in SecurityDBContext.cs).
            menus.Add(SubModule(CoreHRMenuId, "Core HR", "fas fa-id-card nav-icon", 12));
            roleMenus.Add(Grant(RoleMenuId(100), AdminRoleId, CoreHRMenuId, false, false));

            // The User role reaches Leave / Performance through "Human Resources", so it needs that row too.
            roleMenus.Add(Grant(RoleMenuId(101), UserRoleId, HumanResourcesMenuId, false, false));

            // Biometric attendance screens under "Time & Attendance" (MenuId(12)). Fixed ids 200+ so the
            // loop above keeps its numbering; SerialNo orders them after Attendance Summary (33) among
            // their siblings.
            var timeAttendanceMenuId = MenuId(12);
            var biometricScreens = new (string Name, string Route, string Icon)[]
            {
                ("Biometric Registration", "/business/humanresources/attendance/biometric-registrations", "fas fa-fingerprint nav-icon"),
                ("Attendance Punch Log",   "/business/humanresources/attendance/punches",                 "fas fa-list-ol nav-icon"),
                ("Attendance Device",      "/business/humanresources/attendance/devices",                 "fas fa-tablet-alt nav-icon"),
                ("Office Zone",            "/business/humanresources/attendance/zones",                   "fas fa-wifi nav-icon"),
            };
            for (int i = 0; i < biometricScreens.Length; i++)
            {
                var (name, route, icon) = biometricScreens[i];
                var itemId = MenuId(200 + i);
                menus.Add(new AppUserMenu
                {
                    Id = itemId, Name = name, IsHeader = false, IsModule = false, IsSubModule = false, IsComponent = true,
                    CssClass = "nav-item", IsRouteLink = true, RouteLink = route, RouteLinkClass = "nav-link",
                    Icon = icon, Remark = "Navigation Item", ParentId = timeAttendanceMenuId, DropdownIcon = null,
                    SerialNo = 50 + i, CreatedBy = SeedActor, CreatedDate = seedTime, UpdatedDate = seedTime, IsActive = true
                });
                roleMenus.Add(Grant(RoleMenuId(200 + i), AdminRoleId, itemId, true, true));
            }

            modelBuilder.Entity<AppUserMenu>().HasData(menus);
            modelBuilder.Entity<AppUserRoleMenu>().HasData(roleMenus);
        }
        #endregion

        #region Onboarding (was SecurityDBContext.Onboarding.cs)
        // Tables used when HR's EmployeeOnboardedEvent provisions a login:
        // Notifications (navbar bell), EmailOutbox (welcome e-mail), and the
        // AppUserProfiles.EmployeeId link that makes provisioning idempotent — and,
        // for HR's EmployeeOffboardedEvent, the LoginDeactivations schedule.
        public virtual DbSet<Notification>? Notifications { get; set; }
        public virtual DbSet<EmailOutbox>? EmailOutbox { get; set; }
        public virtual DbSet<LoginDeactivation>? LoginDeactivations { get; set; }

        private void ConfigureOnboardingProvisioning(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUserProfile>(e =>
            {
                // one login per employee — also the consumer's idempotency key
                e.HasIndex(x => x.EmployeeId).IsUnique()
                    .HasFilter("[EmployeeId] IS NOT NULL")
                    .HasDatabaseName("UX_AppUserProfiles_EmployeeId");
            });

            modelBuilder.Entity<Notification>(e =>
            {
                e.ToTable("Notifications");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Message).HasMaxLength(1000);
                e.Property(x => x.Type).HasMaxLength(50);
                e.Property(x => x.RouteLink).HasMaxLength(300);
                e.Property(x => x.ReadDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.HasIndex(x => new { x.RecipientProfileId, x.IsRead, x.CreatedDate })
                    .HasDatabaseName("IX_Notifications_Recipient_Unread");
                e.HasOne<AppUserProfile>().WithMany().HasForeignKey(x => x.RecipientProfileId)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Notifications_AppUserProfile");
            });

            modelBuilder.Entity<EmailOutbox>(e =>
            {
                e.ToTable("EmailOutbox");
                e.HasKey(x => x.Id);
                e.Property(x => x.ToAddress).HasMaxLength(300).IsRequired();
                e.Property(x => x.ToName).HasMaxLength(200);
                e.Property(x => x.Subject).HasMaxLength(300).IsRequired();
                e.Property(x => x.Category).HasMaxLength(50);
                e.Property(x => x.LastError).HasMaxLength(2000);
                e.Property(x => x.ProcessedDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.HasIndex(x => new { x.ProcessedDate, x.CreatedDate })
                    .HasFilter("[ProcessedDate] IS NULL")
                    .HasDatabaseName("IX_EmailOutbox_Unprocessed");
            });

            modelBuilder.Entity<LoginDeactivation>(e =>
            {
                e.ToTable("LoginDeactivations");
                e.HasKey(x => x.Id);
                e.Property(x => x.SeparationCode).HasMaxLength(30);
                e.Property(x => x.EmployeeCode).HasMaxLength(30);
                e.Property(x => x.UserName).HasMaxLength(100);
                e.Property(x => x.Outcome).HasMaxLength(30);
                e.Property(x => x.LastWorkingDate).HasColumnType("datetime");
                e.Property(x => x.DeactivateOn).HasColumnType("datetime");
                e.Property(x => x.ProcessedDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.SeparationId).IsUnique().HasDatabaseName("UX_LoginDeactivations_Separation");
                e.HasIndex(x => x.DeactivateOn).HasFilter("[ProcessedDate] IS NULL")
                    .HasDatabaseName("IX_LoginDeactivations_Due");
            });
        }
        #endregion

    }
}
