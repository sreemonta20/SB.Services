﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SB.Security.Persistence;

#nullable disable

namespace SB.Security.Migrations
{
    [DbContext(typeof(SBSecurityDBContext))]
    [Migration("20230715200808_SeedingData")]
    partial class SeedingData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SB.Security.Models.Base.UserInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginAttemptAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("LoginFailedAttemptsCount")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SaltKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("UserInfo");

                    b.HasData(
                        new
                        {
                            Id = new Guid("c047d662-9f0e-4358-b323-15ec3081312c"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9679),
                            Email = "sbhowmikcse08@gmail.com",
                            FullName = "Sreemonta Bhowmik",
                            IsActive = true,
                            LastLoginAttemptAt = new DateTime(2023, 7, 16, 0, 8, 7, 967, DateTimeKind.Local).AddTicks(9668),
                            LoginFailedAttemptsCount = 0,
                            Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9680),
                            UserName = "sree"
                        },
                        new
                        {
                            Id = new Guid("efedc118-3459-4c2e-9158-ad69196a59e0"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9685),
                            Email = "rohine2008@gmail.com",
                            FullName = "Anannya Rohine",
                            IsActive = true,
                            LastLoginAttemptAt = new DateTime(2023, 7, 16, 0, 8, 7, 967, DateTimeKind.Local).AddTicks(9684),
                            LoginFailedAttemptsCount = 0,
                            Password = "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa",
                            RoleId = new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"),
                            SaltKey = "$2b$10$dqPNaHnCGjUcvxXHTRXmDe",
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9685),
                            UserName = "rohine"
                        });
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserLogin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserMenu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CssClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DropdownIcon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsHeader")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RouteLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RouteLinkClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SerialNo")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("UserMenu");

                    b.HasData(
                        new
                        {
                            Id = new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9460),
                            CssClass = "nav-header",
                            Icon = "",
                            IsActive = true,
                            IsHeader = true,
                            Name = "Home",
                            Remark = "Header",
                            RouteLink = "",
                            RouteLinkClass = "",
                            SerialNo = 1,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9461)
                        },
                        new
                        {
                            Id = new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9465),
                            CssClass = "nav-item",
                            Icon = "nav-icon fas fa-tachometer-alt",
                            IsActive = true,
                            IsHeader = false,
                            Name = "Dashboard",
                            Remark = "Navigation Item",
                            RouteLink = "/business/home",
                            RouteLinkClass = "nav-link active",
                            SerialNo = 2,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9465)
                        },
                        new
                        {
                            Id = new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9468),
                            CssClass = "nav-header",
                            Icon = "",
                            IsActive = true,
                            IsHeader = true,
                            Name = "Business",
                            Remark = "Header",
                            RouteLink = "",
                            RouteLinkClass = "",
                            SerialNo = 3,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9469)
                        },
                        new
                        {
                            Id = new Guid("c15215c8-32ca-4182-9510-b57419708a80"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9472),
                            CssClass = "nav-item",
                            DropdownIcon = "fas fa-angle-left right",
                            Icon = "nav-icon fas fa-cog",
                            IsActive = true,
                            IsHeader = false,
                            Name = "Security",
                            Remark = "Navigation Item",
                            RouteLink = "",
                            RouteLinkClass = "nav-link active",
                            SerialNo = 4,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9472)
                        },
                        new
                        {
                            Id = new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9476),
                            CssClass = "nav-item",
                            Icon = "far fa-circle nav-icon",
                            IsActive = true,
                            IsHeader = false,
                            Name = "User",
                            ParentId = new Guid("c15215c8-32ca-4182-9510-b57419708a80"),
                            Remark = "Navigation Item",
                            RouteLink = "/business/security",
                            RouteLinkClass = "nav-link",
                            SerialNo = 5,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9476)
                        },
                        new
                        {
                            Id = new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9543),
                            CssClass = "nav-header",
                            Icon = "",
                            IsActive = true,
                            IsHeader = true,
                            Name = "Settings",
                            Remark = "Header",
                            RouteLink = "",
                            RouteLinkClass = "",
                            SerialNo = 6,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9544)
                        },
                        new
                        {
                            Id = new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9547),
                            CssClass = "nav-item",
                            Icon = "nav-icon fas fa-cog",
                            IsActive = true,
                            IsHeader = false,
                            Name = "App Settings",
                            Remark = "Navigation Item",
                            RouteLink = "/business/appsettings",
                            RouteLinkClass = "nav-link active",
                            SerialNo = 7,
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9547)
                        });
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("UserRole");

                    b.HasData(
                        new
                        {
                            Id = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9324),
                            Description = "Admin",
                            IsActive = true,
                            RoleName = "Admin",
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9330)
                        },
                        new
                        {
                            Id = new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9334),
                            Description = "User",
                            IsActive = true,
                            RoleName = "User",
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9334)
                        });
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserRoleMenu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("MenuId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoleMenu");

                    b.HasData(
                        new
                        {
                            Id = new Guid("fcb6cbdf-bf86-4ec3-b0c2-45dcab597878"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9600),
                            IsActive = true,
                            MenuId = new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9601)
                        },
                        new
                        {
                            Id = new Guid("fb5af66d-ba07-4f88-acd5-e91c38483633"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9605),
                            IsActive = true,
                            MenuId = new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9606)
                        },
                        new
                        {
                            Id = new Guid("39d64f8a-a4c7-4cdb-8be0-785f93c7f01a"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9609),
                            IsActive = true,
                            MenuId = new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9609)
                        },
                        new
                        {
                            Id = new Guid("4379abf7-a1c3-4f39-a60b-043cd57be1e1"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9612),
                            IsActive = true,
                            MenuId = new Guid("c15215c8-32ca-4182-9510-b57419708a80"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9613)
                        },
                        new
                        {
                            Id = new Guid("2d6dffc0-3c90-4a2a-8755-6a07fd6b1aee"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9630),
                            IsActive = true,
                            MenuId = new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9630)
                        },
                        new
                        {
                            Id = new Guid("efd3a617-bce0-49e1-a57b-876d7fa8513e"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9633),
                            IsActive = true,
                            MenuId = new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9634)
                        },
                        new
                        {
                            Id = new Guid("f45cef7d-b805-4f2c-8725-b3eb4a089dde"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9637),
                            IsActive = true,
                            MenuId = new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"),
                            RoleId = new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9637)
                        },
                        new
                        {
                            Id = new Guid("921c8de1-7a7a-409d-816d-30c3768bbc34"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9640),
                            IsActive = true,
                            MenuId = new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"),
                            RoleId = new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9641)
                        },
                        new
                        {
                            Id = new Guid("a270a25f-e808-4071-9dff-285e98c41f73"),
                            CreatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9644),
                            IsActive = true,
                            MenuId = new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"),
                            RoleId = new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"),
                            UpdatedDate = new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9644)
                        });
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserInfo", b =>
                {
                    b.HasOne("SB.Security.Models.Base.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("SB.Security.Models.Base.UserRoleMenu", b =>
                {
                    b.HasOne("SB.Security.Models.Base.UserMenu", "UserMenu")
                        .WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SB.Security.Models.Base.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserMenu");

                    b.Navigation("UserRole");
                });
#pragma warning restore 612, 618
        }
    }
}