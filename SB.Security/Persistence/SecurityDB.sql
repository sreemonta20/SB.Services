USE [SecurityDB]
GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 14.07.2023
-- Description: To generate children menu under particular menu
-- =============================================
CREATE FUNCTION [dbo].[GetChildMenus](@parentId UNIQUEIDENTIFIER, @RoleID UNIQUEIDENTIFIER)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
			 AUM.[Id]
            ,AUM.[Name]
			,AUM.[IsHeader]
			,AUM.[CssClass]
			,AUM.[RouteLink]
			,AUM.[RouteLinkClass]
			,AUM.[Icon]
			,AUM.[Remark]
			,AUM.[ParentId]
			,AUM.[DropdownIcon]
			,AUM.[SerialNo]
			,AUM.[CreatedBy]
			,AUM.[CreatedDate]
			,AUM.[UpdatedBy]
			,AUM.[UpdatedDate]
			,AUM.[IsActive]
			,AURM.[IsView]
			,AURM.[IsCreate]
			,AURM.[IsUpdate]
			,AURM.[IsDelete]
            ,ISNULL(JSON_QUERY(dbo.GetChildMenus(AUM.Id,@RoleID), '$'), '[]') AS Children
        FROM
            AppUserMenus AUM
			INNER JOIN AppUserRoleMenus AURM ON AURM.AppUserMenuId = AUM.Id
			INNER JOIN AppUserRoles AUR ON AUR.Id = AURM.AppUserRoleId
        WHERE
            ParentId = @parentId AND AURM.AppUserRoleId = @RoleId AND AURM.IsActive = 1
        FOR JSON PATH
    )
END
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserMenus]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserMenus](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsHeader] [bit] NULL,
	[CssClass] [nvarchar](100) NULL,
	[RouteLink] [nvarchar](255) NULL,
	[RouteLinkClass] [nvarchar](200) NULL,
	[Icon] [nvarchar](100) NULL,
	[Remark] [nvarchar](255) NULL,
	[ParentId] [uniqueidentifier] NULL,
	[DropdownIcon] [nvarchar](100) NULL,
	[SerialNo] [int] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AppUserMenus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserProfiles]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserProfiles](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](200) NULL,
	[Address] [nvarchar](200) NULL,
	[Email] [nvarchar](200) NULL,
	[AppUserRoleId] [uniqueidentifier] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AppUserProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserRoleMenus]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserRoleMenus](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserRoleId] [uniqueidentifier] NULL,
	[AppUserMenuId] [uniqueidentifier] NULL,
	[IsView] [bit] NULL,
	[IsCreate] [bit] NULL,
	[IsUpdate] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AppUserRoleMenus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserRoles]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](50) NULL,
	[Description] [nvarchar](100) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AppUserRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUsers]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserProfileId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](100) NULL,
	[Password] [nvarchar](255) NULL,
	[SaltKey] [nvarchar](255) NULL,
	[RefreshToken] [nvarchar](255) NULL,
	[RefreshTokenExpiryTime] [datetime] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AppUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecurityLog]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SecurityLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[MessageTemplate] [nvarchar](max) NULL,
	[Level] [nvarchar](max) NULL,
	[TimeStamp] [datetime] NULL,
	[Exception] [nvarchar](max) NULL,
	[Properties] [nvarchar](max) NULL,
 CONSTRAINT [PK_SecurityLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20231124224319_SeedingData', N'7.0.4')
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'52f916cc-6c4d-4b4f-b884-4e89f1489b8d', N'App Settings', 0, N'nav-item', N'/business/appsettings', N'nav-link active', N'nav-icon fas fa-cog', N'Navigation Item', NULL, NULL, 7, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'db0085b7-695d-4751-a190-6c52e3bb44f1', N'Home', 1, N'nav-header', N'', N'', N'', N'Header', NULL, NULL, 1, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'60aadc18-6b91-4cee-ace7-97700b685c98', N'User', 0, N'nav-item', N'/business/security/user', N'nav-link', N'far fa-circle nav-icon', N'Navigation Item', N'c15215c8-32ca-4182-9510-b57419708a80', NULL, 5, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'e8038aef-f00b-4d01-a5d3-99da9cc1a56b', N'Dashboard', 0, N'nav-item', N'/business/home', N'nav-link active', N'nav-icon fas fa-tachometer-alt', N'Navigation Item', NULL, NULL, 2, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'c15215c8-32ca-4182-9510-b57419708a80', N'Security', 0, N'nav-item', N'', N'nav-link active', N'nav-icon fas fa-cog', N'Navigation Item', NULL, N'fas fa-angle-left right', 4, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c', N'Business', 1, N'nav-header', N'', N'', N'', N'Header', NULL, NULL, 3, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserMenus] ([Id], [Name], [IsHeader], [CssClass], [RouteLink], [RouteLinkClass], [Icon], [Remark], [ParentId], [DropdownIcon], [SerialNo], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'52d7e13b-ef24-4f17-937b-d6e8005a6658', N'Settings', 1, N'nav-header', N'', N'', N'', N'Header', NULL, NULL, 6, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserProfiles] ([Id], [FullName], [Address], [Email], [AppUserRoleId], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'c047d662-9f0e-4358-b323-15ec3081312c', N'Sreemonta Bhowmik', N'Dubai', N'sbhowmikcse08@gmail.com', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserProfiles] ([Id], [FullName], [Address], [Email], [AppUserRoleId], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'efedc118-3459-4c2e-9158-ad69196a59e0', N'Anannya Rohine', N'Dubai', N'rohine2008@gmail.com', N'10a9e9e7-cb24-4816-9b94-9db275a40edd', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'698ece21-fd28-44ba-95f1-1ffef963c5b5', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'60aadc18-6b91-4cee-ace7-97700b685c98', 1, 1, 1, 1, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'87f5e973-b247-4fa9-8c48-60f3fd6f2021', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'c15215c8-32ca-4182-9510-b57419708a80', NULL, NULL, NULL, NULL, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'5bca0329-727c-46c1-9c05-7f54292b0b17', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c', NULL, NULL, NULL, NULL, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'd8b9b58f-05d8-4a22-85ac-a2477b2ebaf2', N'10a9e9e7-cb24-4816-9b94-9db275a40edd', N'e8038aef-f00b-4d01-a5d3-99da9cc1a56b', 1, 0, 0, 0, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'8dd17156-46b2-4199-ad17-b32785209468', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'52f916cc-6c4d-4b4f-b884-4e89f1489b8d', 1, 1, 1, 1, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'1f9cea11-53c1-4f42-b9f1-b9189e7203fa', N'10a9e9e7-cb24-4816-9b94-9db275a40edd', N'db0085b7-695d-4751-a190-6c52e3bb44f1', NULL, NULL, NULL, NULL, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'1d3bee1c-6126-4113-8599-c64c7f1d47f6', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'db0085b7-695d-4751-a190-6c52e3bb44f1', NULL, NULL, NULL, NULL, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'f6f1bfa4-1aa9-45e8-876d-cf1c4d959c38', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'e8038aef-f00b-4d01-a5d3-99da9cc1a56b', 1, 1, 1, 1, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoleMenus] ([Id], [AppUserRoleId], [AppUserMenuId], [IsView], [IsCreate], [IsUpdate], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'e7e675a8-f9aa-4def-b952-eee1b18c708a', N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'52d7e13b-ef24-4f17-937b-d6e8005a6658', NULL, NULL, NULL, NULL, N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'1b15ce5a-56b3-4eb9-8286-6e27f770b0da', N'Admin', N'Admin', NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'10a9e9e7-cb24-4816-9b94-9db275a40edd', N'User', N'User', N'1B15CE5A-56B3-4EB9-8286-6E27F770B0DA', CAST(N'2023-11-24T22:43:19.147' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.147' AS DateTime), 1)
INSERT [dbo].[AppUsers] ([Id], [AppUserProfileId], [UserName], [Password], [SaltKey], [RefreshToken], [RefreshTokenExpiryTime], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'5fd67aaf-183e-48f8-bb53-15b7628a3e0a', N'c047d662-9f0e-4358-b323-15ec3081312c', N'sree', N'$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa', N'$2b$10$dqPNaHnCGjUcvxXHTRXmDe', N'qlCFsS0JfvnD6hikhAqgBZ/AbVzX2BVG0UZpLCUYpTQ=', CAST(N'2023-12-25T17:45:38.953' AS DateTime), NULL, CAST(N'2023-11-24T22:43:19.1475728' AS DateTime2), NULL, CAST(N'2023-11-24T22:43:19.1475728' AS DateTime2), 1)
INSERT [dbo].[AppUsers] ([Id], [AppUserProfileId], [UserName], [Password], [SaltKey], [RefreshToken], [RefreshTokenExpiryTime], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'a9e00f47-89ec-46a3-a5e8-31ebd52ac121', N'efedc118-3459-4c2e-9158-ad69196a59e0', N'rohine', N'$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa', N'$2b$10$dqPNaHnCGjUcvxXHTRXmDe', NULL, NULL, NULL, CAST(N'2023-11-24T22:43:19.1475732' AS DateTime2), NULL, CAST(N'2023-11-24T22:43:19.1475732' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[SecurityLog] ON 

INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (1, N'Application is starting', N'Application is starting', N'Information', CAST(N'2023-12-18T21:45:09.570' AS DateTime), NULL, N'<properties><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (2, N'
**********************************************************************
**                      SB Services                                **
**                    [Version 1.0.0]                               **
**  ©2022-2023 Health Care Solutions. All rights reserved           **
**********************************************************************
', N'
**********************************************************************
**                      SB Services                                **
**                    [Version 1.0.0]                               **
**  ©2022-2023 Health Care Solutions. All rights reserved           **
**********************************************************************
', N'Information', CAST(N'2023-12-18T21:45:09.673' AS DateTime), NULL, N'<properties><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (3, N'Login api method started.
', N'Login api method started.
', N'Information', CAST(N'2023-12-18T21:45:35.113' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/Auth/login</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (4, N'Login api method request is: 
{
  "UserName": "sree",
  "Password": "password"
}
', N'Login api method request is: 
{
  "UserName": "sree",
  "Password": "password"
}
', N'Information', CAST(N'2023-12-18T21:45:35.157' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/Auth/login</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (5, N'Authenticate (User service) method request is: 
{
  "UserName": "sree",
  "Password": "password"
}
', N'Authenticate (User service) method request is: 
{
  "UserName": "sree",
  "Password": "password"
}
', N'Information', CAST(N'2023-12-18T21:45:35.240' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/Auth/login</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (6, N'GetAllMenuByUserIdAsync (Role Menu service) method request is: 
"c047d662-9f0e-4358-b323-15ec3081312c"
', N'GetAllMenuByUserIdAsync (Role Menu service) method request is: 
"c047d662-9f0e-4358-b323-15ec3081312c"
', N'Information', CAST(N'2023-12-18T21:45:39.353' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/Auth/login</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (7, N'Login api method response is: 
{
  "Success": true,
  "Message": "Authentation success!",
  "MessageType": 1,
  "ResponseCode": 200,
  "Result": {
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiJjZmZiMTRjZC00MWIyLTRhZWItYmMyNS1lZmQ1NGQxYTQ0NmUiLCJpYXQiOiIxMi8xOC8yMDIzIDU6NDU6MzggUE0iLCJVc2VySWQiOiJjMDQ3ZDY2Mi05ZjBlLTQzNTgtYjMyMy0xNWVjMzA4MTMxMmMiLCJGdWxsTmFtZSI6IlNyZWVtb250YSBCaG93bWlrIiwiVXNlck5hbWUiOiJzcmVlIiwiRW1haWwiOiJzYmhvd21pa2NzZTA4QGdtYWlsLmNvbSIsImV4cCI6MTcwMjkyMTY1OCwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.GUXIDAcexwPujbFNaN5psiEWBh5TsfWBfeCEKkcMKCk",
    "refresh_token": "qlCFsS0JfvnD6hikhAqgBZ/AbVzX2BVG0UZpLCUYpTQ=",
    "expires_in": "2023-12-18T17:47:38.4724209Z",
    "token_type": "bearer",
    "error": "",
    "error_description": "",
    "user": {
      "Id": "c047d662-9f0e-4358-b323-15ec3081312c",
      "FullName": "sree",
      "UserName": "sree",
      "Email": "sbhowmikcse08@gmail.com",
      "UserRole": "1b15ce5a-56b3-4eb9-8286-6e27f770b0da",
      "CreatedDate": "2023-11-24T22:43:19.147"
    },
    "userMenus": "[{\"Id\":\"DB0085B7-695D-4751-A190-6C52E3BB44F1\",\"Name\":\"Home\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":1,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B\",\"Name\":\"Dashboard\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/home\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-tachometer-alt\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":2,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]},{\"Id\":\"F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C\",\"Name\":\"Business\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":3,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"C15215C8-32CA-4182-9510-B57419708A80\",\"Name\":\"Security\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-cog\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":\"fas fa-angle-left right\",\"SerialNo\":4,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[{\"Id\":\"60AADC18-6B91-4CEE-ACE7-97700B685C98\",\"Name\":\"User\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/security\\/user\",\"RouteLinkClass\":\"nav-link\",\"Icon\":\"far fa-circle nav-icon\",\"Remark\":\"Navigation Item\",\"ParentId\":\"C15215C8-32CA-4182-9510-B57419708A80\",\"SerialNo\":5,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]}]},{\"Id\":\"52D7E13B-EF24-4F17-937B-D6E8005A6658\",\"Name\":\"Settings\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":6,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"52F916CC-6C4D-4B4F-B884-4E89F1489B8D\",\"Name\":\"App Settings\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/appsettings\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-cog\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":7,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]}]"
  }
}
', N'Login api method response is: 
{
  "Success": true,
  "Message": "Authentation success!",
  "MessageType": 1,
  "ResponseCode": 200,
  "Result": {
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiJjZmZiMTRjZC00MWIyLTRhZWItYmMyNS1lZmQ1NGQxYTQ0NmUiLCJpYXQiOiIxMi8xOC8yMDIzIDU6NDU6MzggUE0iLCJVc2VySWQiOiJjMDQ3ZDY2Mi05ZjBlLTQzNTgtYjMyMy0xNWVjMzA4MTMxMmMiLCJGdWxsTmFtZSI6IlNyZWVtb250YSBCaG93bWlrIiwiVXNlck5hbWUiOiJzcmVlIiwiRW1haWwiOiJzYmhvd21pa2NzZTA4QGdtYWlsLmNvbSIsImV4cCI6MTcwMjkyMTY1OCwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.GUXIDAcexwPujbFNaN5psiEWBh5TsfWBfeCEKkcMKCk",
    "refresh_token": "qlCFsS0JfvnD6hikhAqgBZ/AbVzX2BVG0UZpLCUYpTQ=",
    "expires_in": "2023-12-18T17:47:38.4724209Z",
    "token_type": "bearer",
    "error": "",
    "error_description": "",
    "user": {
      "Id": "c047d662-9f0e-4358-b323-15ec3081312c",
      "FullName": "sree",
      "UserName": "sree",
      "Email": "sbhowmikcse08@gmail.com",
      "UserRole": "1b15ce5a-56b3-4eb9-8286-6e27f770b0da",
      "CreatedDate": "2023-11-24T22:43:19.147"
    },
    "userMenus": "[{\"Id\":\"DB0085B7-695D-4751-A190-6C52E3BB44F1\",\"Name\":\"Home\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":1,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"E8038AEF-F00B-4D01-A5D3-99DA9CC1A56B\",\"Name\":\"Dashboard\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/home\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-tachometer-alt\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":2,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]},{\"Id\":\"F0F0183B-7F60-4EB4-97D8-D2C15A4AE62C\",\"Name\":\"Business\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":3,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"C15215C8-32CA-4182-9510-B57419708A80\",\"Name\":\"Security\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-cog\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":\"fas fa-angle-left right\",\"SerialNo\":4,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[{\"Id\":\"60AADC18-6B91-4CEE-ACE7-97700B685C98\",\"Name\":\"User\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/security\\/user\",\"RouteLinkClass\":\"nav-link\",\"Icon\":\"far fa-circle nav-icon\",\"Remark\":\"Navigation Item\",\"ParentId\":\"C15215C8-32CA-4182-9510-B57419708A80\",\"SerialNo\":5,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]}]},{\"Id\":\"52D7E13B-EF24-4F17-937B-D6E8005A6658\",\"Name\":\"Settings\",\"IsHeader\":true,\"CssClass\":\"nav-header\",\"RouteLink\":\"\",\"RouteLinkClass\":\"\",\"Icon\":\"\",\"Remark\":\"Header\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":6,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":null,\"IsCreate\":null,\"IsUpdate\":null,\"IsDelete\":null,\"Children\":[]},{\"Id\":\"52F916CC-6C4D-4B4F-B884-4E89F1489B8D\",\"Name\":\"App Settings\",\"IsHeader\":false,\"CssClass\":\"nav-item\",\"RouteLink\":\"\\/business\\/appsettings\",\"RouteLinkClass\":\"nav-link active\",\"Icon\":\"nav-icon fas fa-cog\",\"Remark\":\"Navigation Item\",\"ParentId\":null,\"DropdownIcon\":null,\"SerialNo\":7,\"CreatedBy\":\"C047D662-9F0E-4358-B323-15EC3081312C\",\"CreatedDate\":\"2023-11-24T22:43:19.147\",\"UpdatedBy\":null,\"UpdatedDate\":\"2023-11-24T22:43:19.147\",\"IsActive\":true,\"IsView\":true,\"IsCreate\":true,\"IsUpdate\":true,\"IsDelete\":true,\"Children\":[]}]"
  }
}
', N'Information', CAST(N'2023-12-18T21:45:39.437' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/Auth/login</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (8, N'HTTP "POST" "/api/Auth/login" responded 200 in 5624.7311 ms', N'HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms', N'Information', CAST(N'2023-12-18T21:45:39.490' AS DateTime), NULL, N'<properties><property key=''RequestMethod''>POST</property><property key=''RequestPath''>/api/Auth/login</property><property key=''StatusCode''>200</property><property key=''Elapsed''>5624.7311</property><property key=''SourceContext''>Serilog.AspNetCore.RequestLoggingMiddleware</property><property key=''ActionId''>646cc4cf-2c27-4372-b0ae-ba945bef9027</property><property key=''ActionName''>SB.Security.Controllers.AuthController.Login (SB.Security)</property><property key=''RequestId''>80000002-0001-fc00-b63f-84710c7967bb</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (9, N'GetAllParentMenus api method started.
', N'GetAllParentMenus api method started.
', N'Information', CAST(N'2023-12-18T21:46:24.787' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>79f4c1af-71eb-4bda-8bc4-106850490a4c</property><property key=''ActionName''>SB.Security.Controllers.RoleMenuController.GetAllParentMenus (SB.Security)</property><property key=''RequestId''>80000006-0002-fa00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/RoleMenu/getAllParentMenus</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (10, N'GetAllParentMenus api method request is: 
"Not Applicable"
', N'GetAllParentMenus api method request is: 
"Not Applicable"
', N'Information', CAST(N'2023-12-18T21:46:24.787' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>79f4c1af-71eb-4bda-8bc4-106850490a4c</property><property key=''ActionName''>SB.Security.Controllers.RoleMenuController.GetAllParentMenus (SB.Security)</property><property key=''RequestId''>80000006-0002-fa00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/RoleMenu/getAllParentMenus</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (11, N'GetAllParentMenusAsync (Role Menu service) method request is: 
"Not Applicable"
', N'GetAllParentMenusAsync (Role Menu service) method request is: 
"Not Applicable"
', N'Information', CAST(N'2023-12-18T21:46:24.790' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>79f4c1af-71eb-4bda-8bc4-106850490a4c</property><property key=''ActionName''>SB.Security.Controllers.RoleMenuController.GetAllParentMenus (SB.Security)</property><property key=''RequestId''>80000006-0002-fa00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/RoleMenu/getAllParentMenus</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (12, N'GetAllParentMenus api method response is: 
{
  "Success": true,
  "Message": "Fething parent menu successful!",
  "MessageType": 1,
  "ResponseCode": 200,
  "Result": [
    {
      "Item1": "52f916cc-6c4d-4b4f-b884-4e89f1489b8d",
      "Item2": "App Settings"
    },
    {
      "Item1": "db0085b7-695d-4751-a190-6c52e3bb44f1",
      "Item2": "Home"
    },
    {
      "Item1": "e8038aef-f00b-4d01-a5d3-99da9cc1a56b",
      "Item2": "Dashboard"
    },
    {
      "Item1": "c15215c8-32ca-4182-9510-b57419708a80",
      "Item2": "Security"
    },
    {
      "Item1": "f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c",
      "Item2": "Business"
    },
    {
      "Item1": "52d7e13b-ef24-4f17-937b-d6e8005a6658",
      "Item2": "Settings"
    }
  ]
}
', N'GetAllParentMenus api method response is: 
{
  "Success": true,
  "Message": "Fething parent menu successful!",
  "MessageType": 1,
  "ResponseCode": 200,
  "Result": [
    {
      "Item1": "52f916cc-6c4d-4b4f-b884-4e89f1489b8d",
      "Item2": "App Settings"
    },
    {
      "Item1": "db0085b7-695d-4751-a190-6c52e3bb44f1",
      "Item2": "Home"
    },
    {
      "Item1": "e8038aef-f00b-4d01-a5d3-99da9cc1a56b",
      "Item2": "Dashboard"
    },
    {
      "Item1": "c15215c8-32ca-4182-9510-b57419708a80",
      "Item2": "Security"
    },
    {
      "Item1": "f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c",
      "Item2": "Business"
    },
    {
      "Item1": "52d7e13b-ef24-4f17-937b-d6e8005a6658",
      "Item2": "Settings"
    }
  ]
}
', N'Information', CAST(N'2023-12-18T21:46:24.933' AS DateTime), NULL, N'<properties><property key=''SourceContext''>SB.Security.Service.SecurityLogService</property><property key=''ActionId''>79f4c1af-71eb-4bda-8bc4-106850490a4c</property><property key=''ActionName''>SB.Security.Controllers.RoleMenuController.GetAllParentMenus (SB.Security)</property><property key=''RequestId''>80000006-0002-fa00-b63f-84710c7967bb</property><property key=''RequestPath''>/api/RoleMenu/getAllParentMenus</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
INSERT [dbo].[SecurityLog] ([Id], [Message], [MessageTemplate], [Level], [TimeStamp], [Exception], [Properties]) VALUES (13, N'HTTP "GET" "/api/RoleMenu/getAllParentMenus" responded 200 in 154.5224 ms', N'HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms', N'Information', CAST(N'2023-12-18T21:46:24.937' AS DateTime), NULL, N'<properties><property key=''RequestMethod''>GET</property><property key=''RequestPath''>/api/RoleMenu/getAllParentMenus</property><property key=''StatusCode''>200</property><property key=''Elapsed''>154.5224</property><property key=''SourceContext''>Serilog.AspNetCore.RequestLoggingMiddleware</property><property key=''ActionId''>79f4c1af-71eb-4bda-8bc4-106850490a4c</property><property key=''ActionName''>SB.Security.Controllers.RoleMenuController.GetAllParentMenus (SB.Security)</property><property key=''RequestId''>80000006-0002-fa00-b63f-84710c7967bb</property><property key=''MachineName''>SREE-PC</property><property key=''EnvironmentUserName''>SREE-PC\Sreemonta</property></properties>')
SET IDENTITY_INSERT [dbo].[SecurityLog] OFF
ALTER TABLE [dbo].[AppUserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_AppUserProfiles_AppUserRole] FOREIGN KEY([AppUserRoleId])
REFERENCES [dbo].[AppUserRoles] ([Id])
GO
ALTER TABLE [dbo].[AppUserProfiles] CHECK CONSTRAINT [FK_AppUserProfiles_AppUserRole]
GO
ALTER TABLE [dbo].[AppUserRoleMenus]  WITH CHECK ADD  CONSTRAINT [FK_AppUserRoleMenus_AppUserMenu] FOREIGN KEY([AppUserMenuId])
REFERENCES [dbo].[AppUserMenus] ([Id])
GO
ALTER TABLE [dbo].[AppUserRoleMenus] CHECK CONSTRAINT [FK_AppUserRoleMenus_AppUserMenu]
GO
ALTER TABLE [dbo].[AppUserRoleMenus]  WITH CHECK ADD  CONSTRAINT [FK_AppUserRoleMenus_AppUserRole] FOREIGN KEY([AppUserRoleId])
REFERENCES [dbo].[AppUserRoles] ([Id])
GO
ALTER TABLE [dbo].[AppUserRoleMenus] CHECK CONSTRAINT [FK_AppUserRoleMenus_AppUserRole]
GO
ALTER TABLE [dbo].[AppUsers]  WITH CHECK ADD  CONSTRAINT [FK_AppUser_AppUserProfiles] FOREIGN KEY([AppUserProfileId])
REFERENCES [dbo].[AppUserProfiles] ([Id])
GO
ALTER TABLE [dbo].[AppUsers] CHECK CONSTRAINT [FK_AppUser_AppUserProfiles]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserMenuInitialData]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetUserMenuInitialData
CREATE PROCEDURE [dbo].[SP_GetUserMenuInitialData]
AS
BEGIN
    SET NOCOUNT ON;
	CREATE TABLE #UserMenuInitialDataTable (
        parentMenu XML,
        cssClass XML,
		routeLink XML,
		routeLinkClass XML,
		icon XML,
		dropdownIcon XML
    );

    DECLARE @PMJson XML,
			@CCJson XML,
			@RLJson XML,
			@RLCJson XML,
			@IconJson XML,
			@DDIJson XML,
			@result XML;


    SET @PMJson = (SELECT [Id] id, [Name] name FROM AppUserMenus WHERE [Name] IS NOT NULL FOR JSON AUTO);

	SET @CCJson = (SELECT CssClass id, CssClass name FROM AppUserMenus  WHERE [CssClass] IS NOT NULL FOR JSON AUTO);

	SET @RLJson = (SELECT RouteLink id, RouteLink name FROM AppUserMenus  WHERE [RouteLink] IS NOT NULL FOR JSON AUTO);

	SET @RLCJson = (SELECT RouteLinkClass id, RouteLinkClass name FROM AppUserMenus  WHERE [RouteLinkClass] IS NOT NULL FOR JSON AUTO);

	SET @IconJson = (SELECT Icon id, Icon name FROM AppUserMenus  WHERE [Icon] IS NOT NULL FOR JSON AUTO);

	SET @DDIJson = (SELECT DropdownIcon id, DropdownIcon name FROM AppUserMenus  WHERE [DropdownIcon] IS NOT NULL FOR JSON AUTO);

	INSERT INTO #UserMenuInitialDataTable (parentMenu, cssClass, routeLink, routeLinkClass, icon, dropdownIcon)
    SELECT @PMJson, @CCJson, @RLJson, @RLCJson, @IconJson, @DDIJson

	SET @result = (SELECT * FROM #UserMenuInitialDataTable  FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

	DROP TABLE #UserMenuInitialDataTable;

	SELECT @result AS result;
END;
GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteUser]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 26.04.2023
-- Description:	Delete a user
-- =============================================
--EXEC SP_DeleteUser '10BB4212-AC20-4AC5-A3F6-B5FFF08338C8'
CREATE PROCEDURE [dbo].[SP_DeleteUser]
(
   @Id			UNIQUEIDENTIFIER,
   @IsDelete	BIT
)
AS
BEGIN
	IF (@IsDelete = 1)
	BEGIN
		DELETE FROM [dbo].[AppUserProfiles]
		WHERE Id = @Id
		SELECT @@ROWCOUNT AS 'RowsAffected';
	END
	ELSE
	BEGIN
		UPDATE [dbo].[AppUserProfiles] SET IsActive = 0
		WHERE Id = @Id
		SELECT @@ROWCOUNT AS 'RowsAffected';
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUser]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users
-- =============================================
--EXEC SP_GetAllUser 1, 1
--EXEC SP_GetAllUser 2, 1
--EXEC SP_GetAllUser 1, 2
--EXEC SP_GetAllUser 2, 2
CREATE PROCEDURE [dbo].[SP_GetAllUser] 
	@PageIndex INT, @PageSize INT
	--, @GetTotal BIT, @TotalRecords INT OUTPUT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	--SET @TotalRecords = 0  
	--IF(@GetTotal = 1)  
	--BEGIN  
	--SELECT @TotalRecords = COUNT(Id) FROM UserInfo  
	--END  
	SET @PageIndex = (CASE WHEN @PageIndex = 0 THEN 1 ELSE @PageIndex END)
	SELECT AUP.Id, AUP.FullName,AUP.Address,AUP.Email,AUP.AppUserRoleId,AUP.CreatedBy,AUP.CreatedDate,AUP.UpdatedBy,AUP.UpdatedDate,AUP.IsActive
	FROM AppUserProfiles AUP
	ORDER BY Id ASC OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY  
	RETURN 
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserList]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users list
-- =============================================
--EXEC SP_GetAllUserList 's','Id','ASC',1,2
CREATE     PROCEDURE [dbo].[SP_GetAllUserList]
@SearchTerm AS VARCHAR(50)='',
@SortColumnName AS VARCHAR(50)='',
@SortColumnDirection AS VARCHAR(50)='',
@PageIndex AS INT=0,
@PageSize AS INT=10
AS
BEGIN
	DECLARE @QUERY AS VARCHAR(MAX)='',@ORDER_QUERY AS VARCHAR(MAX)='',@CONDITIONS AS VARCHAR(MAX)='',
	@PAGINATION AS VARCHAR(MAX)=''

	SET @QUERY='SELECT AUP.Id, AUP.FullName,AUP.Address,AUP.Email,AUP.AppUserRoleId,AUP.CreatedBy,AUP.CreatedDate,AUP.UpdatedBy,AUP.UpdatedDate,AUP.IsActive
	FROM AppUserProfiles AUP '

	-- SEARCH OPERATION
	IF(ISNULL(@SearchTerm,'')<>'')
	BEGIN
		IF(ISDATE(@SearchTerm)=1) SET @CONDITIONS=' WHERE CAST(CreatedBy AS DATE)=CAST('+@SearchTerm+'AS DATE)'
		--ELSE IF(ISNUMERIC(@SearchTerm)=1)
		--BEGIN
		--	SET @CONDITIONS=' WHERE salary='+@SEARCH_TEXT+' OR phone_number= CAST('+@SEARCH_TEXT+'AS VARCHAR(50))'
		--END
		ELSE
		BEGIN
			SET @CONDITIONS='
			WHERE
			Id LIKE ''%'+@SearchTerm+'%''
			OR FullName LIKE ''%'+@SearchTerm+'%''
			OR UserName LIKE ''%'+@SearchTerm+'%''
			OR Email LIKE ''%'+@SearchTerm+'%''
			OR RoleId LIKE ''%'+@SearchTerm+'%''
		'
		END
	END

	-- SORT OPERATION
	IF(ISNULL(@SortColumnName,'')<>'' AND ISNULL(@SortColumnDirection,'')<>'')
	BEGIN
		SET @ORDER_QUERY=' ORDER BY '+@SortColumnName+' '+@SortColumnDirection
	END
	ELSE SET @ORDER_QUERY=' ORDER BY Id ASC'

	-- PAGINATION OPERATION
	IF(@PageSize>0)
	BEGIN
		SET @PAGINATION=' OFFSET '+(CAST(((@PageIndex-1)*@PageSize) AS varchar(10)))+' ROWS
		FETCH NEXT '+(CAST(@PageSize AS varchar(10)))+' ROWS ONLY'
	END

	IF(@CONDITIONS<>'') SET @QUERY+=@CONDITIONS
	IF(@ORDER_QUERY<>'') SET @QUERY+=@ORDER_QUERY
	IF(@PAGINATION<>'') SET @QUERY+=@PAGINATION

	--PRINT(@CONDITIONS)
	PRINT(@QUERY)
	EXEC(@QUERY)
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuByUserId]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 14.07.2023
-- Description: To generate parent menu.
-- =============================================
--EXEC SP_GetAllUserMenuByUserId 'C047D662-9F0E-4358-B323-15EC3081312C'
--EXEC SP_GetAllUserMenuByUserId 'EFEDC118-3459-4C2E-9158-AD69196A59E0'
CREATE PROCEDURE [dbo].[SP_GetAllUserMenuByUserId]
@UserId				UNIQUEIDENTIFIER
AS
BEGIN
DECLARE @JsonMenu NVARCHAR(MAX),
		@RoleId UNIQUEIDENTIFIER;
 --   SELECT
 --        [Id]
	--	,[Name]
	--	,[IsHeader]
	--	,[CssClass]
	--	,[RouteLink]
	--	,[RouteLinkClass]
	--	,[Icon]
	--	,[Remark]
	--	,[ParentId]
	--	,[DropdownIcon]
	--	,[SerialNo]
	--	,[CreatedBy]
	--	,[CreatedDate]
	--	,[UpdatedBy]
	--	,[UpdatedDate]
	--	,[IsActive],
 --       dbo.GetChildMenus(AUM.Id) AS Children
 --   FROM
 --       UserMenu AUM
 --   WHERE
 --       ParentId IS NULL
	--ORDER BY SerialNo
 --   FOR JSON PATH, ROOT('UserMenu');
 --	  SELECT @RoleName = UserRole FROM UserInfo UI WHERE  UI.Id =  @UserId
 --	  SELECT @RoleId = UR.Id FROM UserRole UR WHERE  UR.RoleName = @RoleName
	SELECT @RoleId = AppUserRoleId FROM AppUserProfiles UI WHERE  UI.Id =  @UserId
	
	SELECT @JsonMenu = (
        SELECT
         AUM.[Id]
		,AUM.[Name]
		,AUM.[IsHeader]
		,AUM.[CssClass]
		,AUM.[RouteLink]
		,AUM.[RouteLinkClass]
		,AUM.[Icon]
		,AUM.[Remark]
		,AUM.[ParentId]
		,AUM.[DropdownIcon]
		,AUM.[SerialNo]
		,AUM.[CreatedBy]
		,AUM.[CreatedDate]
		,AUM.[UpdatedBy]
		,AUM.[UpdatedDate]
		,AUM.[IsActive]
		,AURM.[IsView]
		,AURM.[IsCreate]
		,AURM.[IsUpdate]
		,AURM.[IsDelete]
        ,ISNULL(JSON_QUERY(dbo.GetChildMenus(AUM.Id,@RoleId), '$'), '[]') AS Children
    FROM
        AppUserMenus AUM
		INNER JOIN AppUserRoleMenus AURM ON AURM.AppUserMenuId = AUM.Id
		INNER JOIN AppUserRoles AUR ON AUR.Id = AURM.AppUserRoleId
		WHERE AURM.AppUserRoleId = @RoleId AND AUM.ParentId IS NULL AND AURM.IsActive = 1
        
	ORDER BY SerialNo
        --FOR JSON AUTO,INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER
		--FOR JSON AUTO,INCLUDE_NULL_VALUES, ROOT('Menu')
		FOR JSON PATH,INCLUDE_NULL_VALUES
    );

    SELECT @JsonMenu AS JsonMenu;
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuPagingWithSearch]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user menu list using paging with search
-- =============================================
--EXEC SP_GetAllUserMenuPagingWithSearch '','','',1,10
--EXEC SP_GetAllUserMenuPagingWithSearch 'nav-icon fas fa-cog','Icon','ASC',1,2
--EXEC SP_GetAllUserMenuPagingWithSearch 'User','','ASC',1,2
CREATE PROCEDURE [dbo].[SP_GetAllUserMenuPagingWithSearch]
@SearchTerm AS VARCHAR(50)='',
@SortColumnName AS VARCHAR(50)='',
@SortColumnDirection AS VARCHAR(50)='',
@PageIndex AS INT=0,
@PageSize AS INT=10
AS
BEGIN
	DECLARE @QUERY AS VARCHAR(MAX)='',@ORDER_QUERY AS VARCHAR(MAX)='',@CONDITIONS AS VARCHAR(MAX)='',
	@PAGINATION AS VARCHAR(MAX)=''

	SET @QUERY='SELECT AUM.Id, AUM.Name,AUM.IsHeader,AUM.CssClass,AUM.RouteLink,AUM.RouteLinkClass,AUM.Icon,AUM.Remark,
	AUM.ParentId,AUM.DropdownIcon,AUM.SerialNo,AUM.CreatedBy,AUM.CreatedDate,AUM.UpdatedBy,AUM.UpdatedDate,AUM.IsActive
	FROM AppUserMenus AUM '

	-- SEARCH OPERATION
	IF(ISNULL(@SearchTerm,'')<>'')
	BEGIN
		IF(ISDATE(@SearchTerm)=1) 
			SET @CONDITIONS=' WHERE CAST(CreatedBy AS DATE)=CAST('+@SearchTerm+'AS DATE)'
		ELSE
		BEGIN
			SET @CONDITIONS='
			WHERE
			Id LIKE ''%'+@SearchTerm+'%''
			OR Name LIKE ''%'+@SearchTerm+'%''
			OR CssClass LIKE ''%'+@SearchTerm+'%''
			OR RouteLink LIKE ''%'+@SearchTerm+'%''
			OR RouteLinkClass LIKE ''%'+@SearchTerm+'%''
			OR Icon LIKE ''%'+@SearchTerm+'%''
			OR Remark LIKE ''%'+@SearchTerm+'%''
			OR DropdownIcon LIKE ''%'+@SearchTerm+'%''
			OR SerialNo LIKE ''%'+@SearchTerm+'%''
		'
		END
	END

	-- SORT OPERATION
	IF(ISNULL(@SortColumnName,'')<>'' AND ISNULL(@SortColumnDirection,'')<>'')
	BEGIN
		SET @ORDER_QUERY=' ORDER BY '+@SortColumnName+' '+@SortColumnDirection
	END
	ELSE SET @ORDER_QUERY=' ORDER BY Id ASC'

	-- PAGINATION OPERATION
	IF(@PageSize>0)
	BEGIN
		SET @PAGINATION=' OFFSET '+(CAST(((@PageIndex-1)*@PageSize) AS varchar(10)))+' ROWS
		FETCH NEXT '+(CAST(@PageSize AS varchar(10)))+' ROWS ONLY'
	END

	IF(@CONDITIONS<>'') SET @QUERY+=@CONDITIONS
	IF(@ORDER_QUERY<>'') SET @QUERY+=@ORDER_QUERY
	IF(@PAGINATION<>'') SET @QUERY+=@PAGINATION

	--PRINT(@CONDITIONS)
	PRINT(@QUERY)
	EXEC(@QUERY)
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserPagingSearch]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllUserPagingSearch 's', 1, 10, ''
CREATE PROCEDURE [dbo].[SP_GetAllUserPagingSearch]
      @SearchTerm VARCHAR(100) = ''
      ,@PageIndex INT = 1
      ,@PageSize INT = 10
      ,@TotalRecords INT OUTPUT
AS
BEGIN
      SET NOCOUNT ON;
      SELECT ROW_NUMBER() OVER
      (
            ORDER BY [Id] ASC
      )AS RowNumber
             ,Id, FullName,Address,Email,AppUserRoleId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive
      INTO #Results
      FROM AppUserProfiles AUP
      WHERE AUP.FullName LIKE '%' + ISNULL(@SearchTerm,'') + '%' OR ISNULL(@SearchTerm,'') = ''
    
      SELECT @TotalRecords = COUNT(*)
      FROM #Results
          
      SELECT Id, FullName,Address,Email,AppUserRoleId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IsActive
      FROM #Results
      WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1
    
      DROP TABLE #Results
END


GO
/****** Object:  StoredProcedure [dbo].[SP_GetMenuHierarchyByMenuId]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 28.07.2023
-- Description: To generate parent menu.
-- =============================================
--EXEC SP_GetMenuHierarchyByMenuId '60AADC18-6B91-4CEE-ACE7-97700B685C98'
--EXEC SP_GetMenuHierarchyByMenuId '52F916CC-6C4D-4B4F-B884-4E89F1489B8D'
CREATE PROCEDURE [dbo].[SP_GetMenuHierarchyByMenuId] 
	@MenuId				UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH UserMenuCTE AS
	(
	     Select	[Id],[Name],[IsHeader] ,[CssClass] ,[RouteLink] ,[RouteLinkClass] ,[Icon] ,[ParentId]
			,[DropdownIcon],[SerialNo] ,[CreatedBy] ,[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive]
		 From [AppUserMenus]
		 Where [Id] = @MenuId
    
		 UNION ALL
    
		 Select	AUM.[Id],AUM.[Name],AUM.[IsHeader] ,AUM.[CssClass] ,AUM.[RouteLink] ,AUM.[RouteLinkClass] ,AUM.[Icon] ,AUM.[ParentId]
			,AUM.[DropdownIcon],AUM.[SerialNo] ,AUM.[CreatedBy] ,AUM.[CreatedDate],AUM.[UpdatedBy],AUM.[UpdatedDate],AUM.[IsActive]
		 From [AppUserMenus] AUM
		 JOIN UserMenuCTE AUMC
		 ON AUM.[Id] = AUMC.[ParentId]
	)

	Select AUMGround.[Id],AUMGround.[Name],AUMGround.[IsHeader] ,AUMGround.[CssClass] ,AUMGround.[RouteLink] ,AUMGround.[RouteLinkClass] ,AUMGround.[Icon], 
	ISNULL(AUMLevel.[Name], 'No Parent') as ParentName
	From UserMenuCTE AUMGround
	LEFT Join UserMenuCTE AUMLevel
	ON AUMGround.[ParentId] = AUMLevel.[Id]
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserById]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 24.04.2023
-- Description:	Get user details by supplying ID
-- =============================================
--EXEC SP_GetUserById 'D670A7BA-F10D-4241-8230-6CD8E0A2B7C0'
CREATE PROCEDURE [dbo].[SP_GetUserById] 
	-- Add the parameters for the stored procedure here
	@Id UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT AUP.Id, AUP.FullName,AUP.Address,AUP.Email,AUP.AppUserRoleId,AUP.CreatedBy,AUP.CreatedDate,AUP.UpdatedBy,AUP.UpdatedDate,AUP.IsActive
	FROM AppUserProfiles AUP 
	WHERE AUP.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_SaveUpdateAppUser]
	@ActionName					VARCHAR(10), --Save Update
    @Id							UNIQUEIDENTIFIER,
	@AppUserProfileId			UNIQUEIDENTIFIER,
    @UserName					NVARCHAR(50),
	@Password					NVARCHAR(MAX),
	@SaltKey					NVARCHAR(MAX),
	@RefreshToken				NVARCHAR(MAX),
	@RefreshTokenExpiryTime		DATETIME,
	@CreatedBy					NVARCHAR(MAX),
	@CreatedDate				DATETIME2(7),
	@UpdatedBy					NVARCHAR(MAX),
	@UpdatedDate				DATETIME2(7),
	@IsActive					BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ActionName = 'Save' -- Save
    BEGIN
        INSERT INTO AppUsers ([Id],[AppUserProfileId],[UserName],[Password],[SaltKey],[RefreshToken],[RefreshTokenExpiryTime],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive])
     VALUES
           (@Id,@AppUserProfileId, @UserName,@Password, @SaltKey, @RefreshToken, @RefreshTokenExpiryTime, @CreatedBy,@CreatedDate,NULL, NULL, @IsActive)

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update' -- Update
    BEGIN
        UPDATE AppUsers SET [Id] = @Id,[AppUserProfileId] = @AppUserProfileId,[UserName] = @UserName,[Password] = @Password,[SaltKey] = @SaltKey,[RefreshToken] = @RefreshToken,
		[RefreshTokenExpiryTime] = @RefreshTokenExpiryTime, [UpdatedBy] = @UpdatedBy,[UpdatedDate] = @UpdatedDate, IsActive = @IsActive
        WHERE [Id] = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
    BEGIN
        RAISERROR('Invalid action flag. Must be either ''Save'' or ''Update''.', 16, 1);
    END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUserProfile]    Script Date: 12/24/2023 2:23:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_SaveUpdateAppUserProfile]
	@ActionName     VARCHAR(10), --Save Update
    @Id				UNIQUEIDENTIFIER,
    @FullName		NVARCHAR(200),
    @Address		NVARCHAR(200),
	@Email			VARCHAR(200),
	@RoleId			UNIQUEIDENTIFIER,
	@CreatedBy		NVARCHAR(MAX),
	@CreatedDate	DATETIME2(7),
	@UpdatedBy		NVARCHAR(MAX),
	@UpdatedDate	DATETIME2(7),
	@IsActive		BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ActionName = 'Save' -- Save
    BEGIN
        INSERT INTO AppUserProfiles ([Id],[FullName],[Address],[Email],[AppUserRoleId],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive])
     VALUES
           (@Id,@FullName, @Address,@Email, @RoleId, @CreatedBy,@CreatedDate,NULL, NULL, @IsActive)

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update' -- Update
    BEGIN
        UPDATE AppUserProfiles SET [FullName] = @FullName, [Address] = @Address, [Email] = @Email,[AppUserRoleId] = @RoleId
		, [UpdatedBy] = @UpdatedBy,[UpdatedDate] = @UpdatedDate, IsActive = @IsActive
        WHERE [Id] = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
    BEGIN
        RAISERROR('Invalid action flag. Must be either ''Save'' or ''Update''.', 16, 1);
    END
END
GO
