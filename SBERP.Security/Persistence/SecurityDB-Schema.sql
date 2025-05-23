USE [master]
GO
/****** Object:  Database [SecurityDB]    Script Date: 4/20/2025 8:49:07 PM ******/
CREATE DATABASE [SecurityDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SecurityDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL16\MSSQL\DATA\SecurityDB.mdf' , SIZE = 270336KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SecurityDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL16\MSSQL\DATA\SecurityDB_log.ldf' , SIZE = 204800KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [SecurityDB] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SecurityDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SecurityDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SecurityDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SecurityDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SecurityDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SecurityDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SecurityDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SecurityDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SecurityDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SecurityDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SecurityDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SecurityDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SecurityDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SecurityDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SecurityDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SecurityDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SecurityDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SecurityDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SecurityDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SecurityDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SecurityDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SecurityDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SecurityDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SecurityDB] SET RECOVERY FULL 
GO
ALTER DATABASE [SecurityDB] SET  MULTI_USER 
GO
ALTER DATABASE [SecurityDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SecurityDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SecurityDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SecurityDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SecurityDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'SecurityDB', N'ON'
GO
ALTER DATABASE [SecurityDB] SET QUERY_STORE = OFF
GO
USE [SecurityDB]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [SecurityDB]
GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 4/20/2025 8:49:07 PM ******/
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
			,AUM.[IsModule]
			,AUM.[IsComponent]
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Table [dbo].[AppUserMenus]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserMenus](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsHeader] [bit] NULL,
	[IsModule] [bit] NULL,
	[IsComponent] [bit] NULL,
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
/****** Object:  Table [dbo].[AppUserMenusLog]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserMenusLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserMenuId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsHeader] [bit] NULL,
	[IsModule] [bit] NULL,
	[IsComponent] [bit] NULL,
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
	[PerformedUser] [nvarchar](max) NULL,
	[Action] [nvarchar](200) NULL,
 CONSTRAINT [PK_AppUserMenusLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserProfiles]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Table [dbo].[AppUserProfilesLog]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserProfilesLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserProfileId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](200) NULL,
	[Address] [nvarchar](200) NULL,
	[Email] [nvarchar](200) NULL,
	[AppUserRoleId] [uniqueidentifier] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[PerformedUser] [nvarchar](max) NULL,
	[Action] [nvarchar](200) NULL,
 CONSTRAINT [PK_AppUserProfilesLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserRoleMenus]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Table [dbo].[AppUserRoleMenusLog]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserRoleMenusLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserRoleMenuId] [uniqueidentifier] NOT NULL,
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
	[PerformedUser] [nvarchar](max) NULL,
	[Action] [nvarchar](200) NULL,
 CONSTRAINT [PK_AppUserRoleMenusLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUserRoles]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Table [dbo].[AppUserRolesLog]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUserRolesLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserRoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](50) NULL,
	[Description] [nvarchar](100) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[PerformedUser] [nvarchar](max) NULL,
	[Action] [nvarchar](200) NULL,
 CONSTRAINT [PK_AppUserRolesLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppUsers]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Table [dbo].[AppUsersLog]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUsersLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AppUserId] [uniqueidentifier] NOT NULL,
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
	[PerformedUser] [nvarchar](max) NULL,
	[Action] [nvarchar](200) NULL,
 CONSTRAINT [PK_AppUsersLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecurityLog]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  Index [IX_AppUserProfiles_AppUserRoleId]    Script Date: 4/20/2025 8:49:07 PM ******/
CREATE NONCLUSTERED INDEX [IX_AppUserProfiles_AppUserRoleId] ON [dbo].[AppUserProfiles]
(
	[AppUserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AppUserRoleMenus_AppUserMenuId]    Script Date: 4/20/2025 8:49:07 PM ******/
CREATE NONCLUSTERED INDEX [IX_AppUserRoleMenus_AppUserMenuId] ON [dbo].[AppUserRoleMenus]
(
	[AppUserMenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AppUserRoleMenus_AppUserRoleId]    Script Date: 4/20/2025 8:49:07 PM ******/
CREATE NONCLUSTERED INDEX [IX_AppUserRoleMenus_AppUserRoleId] ON [dbo].[AppUserRoleMenus]
(
	[AppUserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AppUsers_AppUserProfileId]    Script Date: 4/20/2025 8:49:07 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AppUsers_AppUserProfileId] ON [dbo].[AppUsers]
(
	[AppUserProfileId] ASC
)
WHERE ([AppUserProfileId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
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
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserMenu]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 21.04.2024
-- Description:	This SP is used to create or update User Menu
-- =============================================
CREATE PROCEDURE [dbo].[SP_CreateUpdateAppUserMenu]
	-- Add the parameters for the stored procedure here
	@ActionName			VARCHAR(10), --Save Update
    @Id					UNIQUEIDENTIFIER,
    @Name				NVARCHAR(100),
    @IsHeader			BIT,
	@IsModule			BIT,
	@IsComponent		BIT,
	@CssClass			NVARCHAR(100),
	@RouteLink			NVARCHAR(255),
	@RouteLinkClass		NVARCHAR(200),
	@Icon				NVARCHAR(100),
	@Remark				NVARCHAR(255),
	@ParentId			UNIQUEIDENTIFIER,
	@DropdownIcon		NVARCHAR(100),
	@SerialNo			INT,
	@CreatedBy			NVARCHAR(MAX),
	@CreatedDate		DATETIME2(7),
	@UpdatedBy			NVARCHAR(MAX),
	@UpdatedDate		DATETIME2(7),
	@IsActive			BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @ActionName = 'Save' -- Save
    BEGIN
		IF EXISTS (SELECT 1 FROM AppUserMenus WHERE Id = @Id)
		BEGIN
			SELECT 0 AS 'RowsAffected';
		END
		ELSE
		BEGIN
			INSERT INTO AppUserMenus (Id, Name, IsHeader, IsModule, IsComponent, CssClass, RouteLink, RouteLinkClass, Icon, Remark, ParentId, DropdownIcon, SerialNo, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive)
			VALUES (@Id, @Name, @IsHeader, @IsModule, @IsComponent, @CssClass, @RouteLink, @RouteLinkClass, @Icon, @Remark, @ParentId, @DropdownIcon, @SerialNo, @CreatedBy, GETUTCDATE(), NULL, NULL, @IsActive);

			SELECT @@ROWCOUNT AS 'RowsAffected';
		END
        
    END
	ELSE IF @ActionName = 'Update' -- Update
    BEGIN
		IF EXISTS (SELECT 1 FROM AppUserMenus WHERE Id = @Id)
		BEGIN
			UPDATE AppUserMenus SET Name = @Name, IsHeader = @IsHeader,  IsModule = @IsModule, IsComponent = @IsComponent, CssClass = @CssClass, RouteLink = @RouteLink,
			RouteLinkClass = @RouteLinkClass, Icon = @Icon, Remark = @Remark, ParentId = @ParentId,DropdownIcon = @DropdownIcon,
			SerialNo = @SerialNo,UpdatedBy = @UpdatedBy,UpdatedDate = GETUTCDATE(),IsActive = @IsActive
			WHERE [Id] = @Id;
		
			SELECT @@ROWCOUNT AS 'RowsAffected';
		END
		ELSE
		BEGIN
			SELECT 0 AS 'RowsAffected';
		END
    END
    ELSE
    BEGIN
        RAISERROR('Invalid action flag. Must be either ''Save'' or ''Update''.', 16, 1);
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserRoleMenu]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 28.04.2024
-- Description:	This SP is used to assign application user menus to a role or update the application user menus to a certain role.
-- =============================================
CREATE PROCEDURE [dbo].[SP_CreateUpdateAppUserRoleMenu]
	-- Add the parameters for the stored procedure here
	@ActionName			VARCHAR(10), --Save Update
    @Id					UNIQUEIDENTIFIER,
    @AppUserRoleId		UNIQUEIDENTIFIER,
    @AppUserMenuId		UNIQUEIDENTIFIER,
    @IsView				BIT,
    @IsCreate			BIT,
    @IsUpdate			BIT,
    @IsDelete			BIT,
    @CreatedBy			NVARCHAR(MAX),
	@CreatedDate		DATETIME2(7),
	@UpdatedBy			NVARCHAR(MAX),
	@UpdatedDate		DATETIME2(7),
    @IsActive			BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @ActionName = 'Save' -- Save
    BEGIN
		IF NOT EXISTS (SELECT 1 FROM [dbo].[AppUserRoleMenus] WHERE [Id] = @Id)
		BEGIN
			SELECT 0 AS 'RowsAffected';
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[AppUserRoleMenus] ([Id],[AppUserRoleId],[AppUserMenuId], [IsView], [IsCreate],[IsUpdate],[IsDelete],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive]) 
			VALUES (@Id,@AppUserRoleId,@AppUserMenuId,@IsView,@IsCreate,@IsUpdate,@IsDelete,@CreatedBy,GETUTCDATE(), NULL, NULL, @IsActive);

			SELECT @@ROWCOUNT AS 'RowsAffected';
		END
        
    END
	ELSE IF @ActionName = 'Update' -- Update
    BEGIN
		IF EXISTS (SELECT 1 FROM [dbo].[AppUserRoleMenus] WHERE [Id] = @Id)
		BEGIN
			UPDATE [dbo].[AppUserRoleMenus] SET [AppUserRoleId] = @AppUserRoleId,[AppUserMenuId] = @AppUserMenuId,[IsView] = @IsView,[IsCreate] = @IsCreate,
					[IsUpdate] = @IsUpdate,[IsDelete] = @IsDelete,[UpdatedBy] = @UpdatedBy,[UpdatedDate] = GETUTCDATE(),[IsActive] = @IsActive
			WHERE [Id] = @Id;
		
			SELECT @@ROWCOUNT AS 'RowsAffected';
		END
		ELSE
		BEGIN
			SELECT 0 AS 'RowsAffected';
		END
    END
    ELSE
    BEGIN
        RAISERROR('Invalid action flag. Must be either ''Save'' or ''Update''.', 16, 1);
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserMenu]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 22nd April 2024
-- Description:	This Stored Procedure is to delete or inactive based on @IsDelete flag parameter supplied value.
-- =============================================
--EXEC SP_DeleteAppUserMenu 0,'60AADC18-6B91-4CEE-ACE7-97700B685C98'
CREATE PROCEDURE [dbo].[SP_DeleteAppUserMenu]
    @IsDelete BIT,
    @MenuId NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @ConvertedMenuId UNIQUEIDENTIFIER;
    DECLARE @RowCount INT;
	DECLARE @AffectedRow INT = 0;
	DECLARE @Result NVARCHAR(MAX);

	SET @ConvertedMenuId = CAST(@MenuId AS UNIQUEIDENTIFIER);
    -- Check if the MenuId is used in UserRoleMenu table
    SELECT @RowCount = COUNT(*)
    FROM AppUserRoleMenus
    WHERE AppUserMenuId = @ConvertedMenuId AND IsActive = 1;

    IF @RowCount > 0
    BEGIN
        -- Menu is already used in UserRoleMenu, return JSON message
        --SELECT '{"rowcount": 0, "message": "This menu already used for a role"}' AS Result;
		SET @Result = N'{"message": "This menu already used for a role", "rowcount": ' + CAST(@AffectedRow AS NVARCHAR(10)) + ', "sucess":"false"}';
		SELECT @Result
    END
    ELSE
    BEGIN
        -- Menu is not used in UserRoleMenu
        IF @IsDelete = 1
        BEGIN
            -- Delete the UserMenu record
            DELETE FROM AppUserMenus WHERE Id = @ConvertedMenuId;
			SET @AffectedRow = @@ROWCOUNT;
			SET @Result = N'{"message": "User menu is successfully removed", "rowcount": ' + CAST(@AffectedRow AS NVARCHAR(10)) + ', "sucess":"true"}';
            --SELECT '{"rowcount": 1, "message": "User menu is successfully removed"}' AS Result;
			SELECT @Result
        END
        ELSE
        BEGIN
            -- Update IsActive column to false
            UPDATE AppUserMenus SET IsActive = 0 WHERE Id = @ConvertedMenuId;
			SET @AffectedRow = @@ROWCOUNT;
			SET @Result = N'{"message": "User menu is successfully inactivated", "rowcount": ' + CAST(@AffectedRow AS NVARCHAR(10)) + ', "sucess":"true"}';
			--SELECT '{"rowcount": 1, "message": "User menu is successfully inactivated"}' AS Result;
			SELECT @Result
        END
    END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserProfile]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 26.04.2023
-- Description:	Delete a user
-- =============================================
--EXEC SP_DeleteAppUserProfile '10BB4212-AC20-4AC5-A3F6-B5FFF08338C8'
CREATE PROCEDURE [dbo].[SP_DeleteAppUserProfile]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusByUserId]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 14.07.2023
-- Description: To generate parent menu.
-- =============================================
--EXEC SP_GetAllAppUserMenusByUserId 'C047D662-9F0E-4358-B323-15EC3081312C'
--EXEC SP_GetAllAppUserMenusByUserId 'EFEDC118-3459-4C2E-9158-AD69196A59E0'
CREATE PROCEDURE [dbo].[SP_GetAllAppUserMenusByUserId]
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
		,AUM.[IsModule]
		,AUM.[IsComponent]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusPagingWithSearch]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllAppUserMenusPagingWithSearch '','','',1,10
--EXEC SP_GetAllAppUserMenusPagingWithSearch 'nav-icon fas fa-cog','Icon','ASC',1,2
--EXEC SP_GetAllAppUserMenusPagingWithSearch 1,5,'User','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearch 1,5,'','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearch 1,10,'','Name','asc'
CREATE PROCEDURE [dbo].[SP_GetAllAppUserMenusPagingWithSearch]
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm VARCHAR(50) = '',
    @SortColumnName VARCHAR(50) = '',
    @SortColumnDirection VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRecords INT;
    DECLARE @TotalPages INT;
    DECLARE @Result XML;
    DECLARE @Items XML;

    CREATE TABLE #AppUserMenuTBL
    (
        [RowCount] INT,
        [CurrentPage] INT,
        [PageSize] INT,
        [PageCount] INT,
        [Items] XML
    );

    -- Calculate total records with the search term filter
    SELECT @TotalRecords = COUNT(*)
    FROM [dbo].[AppUserMenus]
    WHERE IsActive = 1
          AND (
                  @SearchTerm = ''
                  OR [Name] LIKE '%' + @SearchTerm + '%'
              );

    -- Calculate total pages
    SET @TotalPages = CEILING(CAST(@TotalRecords AS FLOAT) / @PageSize);

    WITH SortedAppUserMenus AS
    (
        SELECT 
            am.[Id],
            am.[Name],
            am.[IsHeader],
            am.[IsModule],
            am.[IsComponent],
            am.[CssClass],
            am.[RouteLink],
            am.[RouteLinkClass],
            am.[Icon],
            am.[Remark],
            am.[ParentId],
            CASE 
				WHEN am.[ParentId] IS NOT NULL THEN 
					(SELECT [Name] FROM [dbo].[AppUserMenus] WHERE [Id] = am.[ParentId])
				ELSE ''
			END AS ParentName,
            am.[DropdownIcon],
            am.[SerialNo],
            am.[CreatedBy],
            am.[CreatedDate],
            am.[UpdatedBy],
            am.[UpdatedDate],
            am.[IsActive],
            ROW_NUMBER() OVER 
            (
                ORDER BY 
					CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'asc' THEN am.[Name] END ASC,
					CASE WHEN @SortColumnName = 'IsHeader' AND @SortColumnDirection = 'asc' THEN am.[IsHeader] END ASC,
					CASE WHEN @SortColumnName = 'IsModule' AND @SortColumnDirection = 'asc' THEN am.[IsModule] END ASC,
					CASE WHEN @SortColumnName = 'IsComponent' AND @SortColumnDirection = 'asc' THEN am.[IsComponent] END ASC,
					CASE WHEN @SortColumnName = 'RouteLink' AND @SortColumnDirection = 'asc' THEN am.[RouteLink] END ASC,
					CASE WHEN @SortColumnName = 'ParentName' AND @SortColumnDirection = 'asc' THEN 
						CASE WHEN am.[ParentId] IS NOT NULL THEN 
							(SELECT [Name] FROM [dbo].[AppUserMenus] WHERE [Id] = am.[ParentId])
						ELSE '' 
						END 
					END ASC,
					CASE WHEN @SortColumnName = 'IsActive' AND @SortColumnDirection = 'asc' THEN am.[IsActive] END ASC,
                    CASE WHEN @SortColumnName = 'SerialNo' AND @SortColumnDirection = 'asc' THEN am.[SerialNo] END ASC,
                    
                    CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'desc' THEN am.[Name] END DESC,
					CASE WHEN @SortColumnName = 'IsHeader' AND @SortColumnDirection = 'desc' THEN am.[IsHeader] END DESC,
					CASE WHEN @SortColumnName = 'IsModule' AND @SortColumnDirection = 'desc' THEN am.[IsModule] END DESC,
					CASE WHEN @SortColumnName = 'IsComponent' AND @SortColumnDirection = 'desc' THEN am.[IsComponent] END DESC,
					CASE WHEN @SortColumnName = 'RouteLink' AND @SortColumnDirection = 'desc' THEN am.[RouteLink] END DESC,
					CASE WHEN @SortColumnName = 'ParentName' AND @SortColumnDirection = 'desc' THEN 
						CASE WHEN am.[ParentId] IS NOT NULL THEN 
							(SELECT [Name] FROM [dbo].[AppUserMenus] WHERE [Id] = am.[ParentId])
						ELSE '' 
						END 
					END DESC,
					CASE WHEN @SortColumnName = 'IsActive' AND @SortColumnDirection = 'desc' THEN am.[IsActive] END DESC,
                    CASE WHEN @SortColumnName = 'SerialNo' AND @SortColumnDirection = 'desc' THEN am.[SerialNo] END DESC,

                    -- Default sort when nothing matches or as a tiebreaker
                    am.[SerialNo] ASC
            ) AS RowNum
        FROM 
            [dbo].[AppUserMenus] am
        WHERE 
            am.IsActive = 1
            AND (
                @SearchTerm = ''
                OR am.[Name] LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT @Items =
    (
        SELECT 
            [Id],
            [Name],
            [IsHeader],
            [IsModule],
            [IsComponent],
            [CssClass],
            [RouteLink],
            [RouteLinkClass],
            [Icon],
            [Remark],
            [ParentId],
            [ParentName],
            [DropdownIcon],
            [SerialNo],
            [CreatedBy],
            [CreatedDate],
            [UpdatedBy],
            [UpdatedDate],
            [IsActive]
        FROM 
            SortedAppUserMenus
        WHERE 
            RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #AppUserMenuTBL
    (
        [RowCount],
        [CurrentPage],
        [PageSize],
        [PageCount],
        [Items]
    )
    SELECT @TotalRecords,
           @PageNumber,
           @PageSize,
           @TotalPages,
           @Items;

    SET @Result =
    (
        SELECT * FROM #AppUserMenuTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DROP TABLE #AppUserMenuTBL;

    SELECT @Result AS result;
END;
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfileList]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users list
-- =============================================
--EXEC SP_GetAllAppUserProfileList 's','Id','ASC',1,2
CREATE     PROCEDURE [dbo].[SP_GetAllAppUserProfileList]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfiles]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users
-- =============================================
--EXEC SP_GetAllAppUserProfiles 1, 1
--EXEC SP_GetAllAppUserProfiles 2, 1
--EXEC SP_GetAllAppUserProfiles 1, 2
--EXEC SP_GetAllAppUserProfiles 2, 2
CREATE PROCEDURE [dbo].[SP_GetAllAppUserProfiles] 
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfilesPagingSearch]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllAppUserProfilesPagingSearch 's', 1, 10, ''
CREATE PROCEDURE [dbo].[SP_GetAllAppUserProfilesPagingSearch]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserRoleMenusPagingWithSearch]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user role menu list using paging with search
-- =============================================
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch '','','',1,10
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch 'User','','ASC',1,2
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch 'Admin','','ASC',1,2 
CREATE PROCEDURE [dbo].[SP_GetAllAppUserRoleMenusPagingWithSearch]
@SearchTerm AS VARCHAR(50)='',
@SortColumnName AS VARCHAR(50)='',
@SortColumnDirection AS VARCHAR(50)='',
@PageIndex AS INT=0,
@PageSize AS INT=10
AS
BEGIN
	DECLARE @QUERY AS VARCHAR(MAX)='',@ORDER_QUERY AS VARCHAR(MAX)='',@CONDITIONS AS VARCHAR(MAX)='',
	@PAGINATION AS VARCHAR(MAX)=''

	SET @QUERY='SELECT AURM.Id  AS Id, AURM.AppUserRoleId, AUR.RoleName, AURM.AppUserMenuId, AUM.Name AS [MenuName],AURM.IsView,AURM.IsCreate,AURM.IsUpdate,AURM.IsDelete,AURM.CreatedBy,AURM.CreatedDate,AURM.UpdatedBy,AURM.UpdatedDate,AURM.IsActive
	FROM AppUserRoleMenus AURM 
	INNER JOIN AppUserRoles AUR ON AUR.Id = AURM.AppUserRoleId
	INNER JOIN AppUserMenus AUM ON AUM.Id = AURM.AppUserMenuId'

	-- SEARCH OPERATION
	IF(ISNULL(@SearchTerm,'')<>'')
	BEGIN
		IF(ISDATE(@SearchTerm)=1) 
			SET @CONDITIONS=' WHERE CAST(CreatedBy AS DATE)=CAST('+@SearchTerm+'AS DATE)'
		ELSE
		BEGIN
			SET @CONDITIONS='
			WHERE
			AURM.Id LIKE ''%'+@SearchTerm+'%''
			OR AUR.RoleName LIKE ''%'+@SearchTerm+'%''
			OR AUM.Name LIKE ''%'+@SearchTerm+'%''
		'
		END
	END

	-- SORT OPERATION
	IF(ISNULL(@SortColumnName,'')<>'' AND ISNULL(@SortColumnDirection,'')<>'')
	BEGIN
		SET @ORDER_QUERY=' ORDER BY '+@SortColumnName+' '+@SortColumnDirection
	END
	ELSE SET @ORDER_QUERY=' ORDER BY AURM.Id ASC'

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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserMenuHierarchyByMenuId]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 28.07.2023
-- Description: To generate parent menu.
-- =============================================
--EXEC SP_GetAppUserMenuHierarchyByMenuId '60AADC18-6B91-4CEE-ACE7-97700B685C98'
--EXEC SP_GetAppUserMenuHierarchyByMenuId '52F916CC-6C4D-4B4F-B884-4E89F1489B8D'
CREATE PROCEDURE [dbo].[SP_GetAppUserMenuHierarchyByMenuId] 
	@MenuId				UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH UserMenuCTE AS
	(
	     Select	[Id],[Name],[IsHeader] ,[IsModule] ,[IsComponent] ,[CssClass] ,[RouteLink] ,[RouteLinkClass] ,[Icon] ,[ParentId]
			,[DropdownIcon],[SerialNo] ,[CreatedBy] ,[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive]
		 From [AppUserMenus]
		 Where [Id] = @MenuId
    
		 UNION ALL
    
		 Select	AUM.[Id],AUM.[Name],AUM.[IsHeader] ,AUM.[IsModule] ,AUM.[IsComponent] ,AUM.[CssClass] ,AUM.[RouteLink] ,AUM.[RouteLinkClass] ,AUM.[Icon] ,AUM.[ParentId]
			,AUM.[DropdownIcon],AUM.[SerialNo] ,AUM.[CreatedBy] ,AUM.[CreatedDate],AUM.[UpdatedBy],AUM.[UpdatedDate],AUM.[IsActive]
		 From [AppUserMenus] AUM
		 JOIN UserMenuCTE AUMC
		 ON AUM.[Id] = AUMC.[ParentId]
	)

	Select AUMGround.[Id],AUMGround.[Name],AUMGround.[IsHeader] ,AUMGround.[IsModule], AUMGround.[IsComponent] , AUMGround.[CssClass] ,AUMGround.[RouteLink] ,AUMGround.[RouteLinkClass] ,AUMGround.[Icon], 
	ISNULL(AUMLevel.[Name], 'No Parent') as ParentName
	From UserMenuCTE AUMGround
	LEFT Join UserMenuCTE AUMLevel
	ON AUMGround.[ParentId] = AUMLevel.[Id]
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserProfileById]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 24.04.2023
-- Description:	Get user details by supplying ID
-- =============================================
--EXEC SP_GetAppUserProfileById 'D670A7BA-F10D-4241-8230-6CD8E0A2B7C0'
CREATE PROCEDURE [dbo].[SP_GetAppUserProfileById] 
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserRoleMenuInitialData]    Script Date: 4/20/2025 8:49:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAppUserRoleMenuInitialData
CREATE PROCEDURE [dbo].[SP_GetAppUserRoleMenuInitialData]
AS
BEGIN
    SET NOCOUNT ON;
    CREATE TABLE #AppUserRoleMenuInitTBL
    (
		userRolesList		XML,
        parentMenuList		XML,
        cssClassList		XML,
        routeLinkList		XML,
        routeLinkClassList	XML,
        iconList			XML,
        dropdownIconList	XML,
		nextMenuSlNo		INT
    );

    DECLARE @URJson			XML,
			@PMJson			XML,
            @CCJson			XML,
            @RLJson			XML,
            @RLCJson		XML,
            @IconJson		XML,
            @DDIJson		XML,
            @result			XML,
			@nextMenuSlNo	INT;

	SET @URJson =
    (
        SELECT DISTINCT [Id] id,
               [RoleName] name
        FROM AppUserRoles
        WHERE [RoleName] IS NOT NULL
        FOR JSON AUTO
    );

    SET @PMJson =
    (
        SELECT DISTINCT [Id] id,
               [Name] + CASE
                            WHEN [IsHeader] = 1
                                 AND [IsModule] = 0
                                 AND [IsComponent] = 0 THEN
                                ' (Header Menu)'
                            WHEN [IsHeader] = 0
                                 AND [IsModule] = 1
                                 AND [IsComponent] = 0 THEN
                                ' (Module Menu)'
                            ELSE
                                ' (Business Menu)'
                        END AS name
        FROM AppUserMenus
        WHERE [Name] IS NOT NULL
        FOR JSON AUTO
    );

    SET @CCJson =
    (
        SELECT DISTINCT CssClass id,
               CssClass name
        FROM AppUserMenus
        WHERE [CssClass] IS NOT NULL
        FOR JSON AUTO
    );

    SET @RLJson =
    (
        SELECT DISTINCT RouteLink id,
               RouteLink name
        FROM AppUserMenus
        WHERE [RouteLink] IS NOT NULL
        FOR JSON AUTO
    );

    SET @RLCJson =
    (
        SELECT DISTINCT RouteLinkClass id,
               RouteLinkClass name
        FROM AppUserMenus
        WHERE [RouteLinkClass] IS NOT NULL
        FOR JSON AUTO
    );

    SET @IconJson =
    (
        SELECT DISTINCT Icon id,
               Icon name
        FROM AppUserMenus
        WHERE [Icon] IS NOT NULL
        FOR JSON AUTO
    );

    SET @DDIJson =
    (
        SELECT DISTINCT DropdownIcon id,
               DropdownIcon name
        FROM AppUserMenus
        WHERE [DropdownIcon] IS NOT NULL
        FOR JSON AUTO
    );

	SET @nextMenuSlNo = (SELECT  MAX(SerialNo)  AS SerialNo FROM AppUserMenus)

    INSERT INTO #AppUserRoleMenuInitTBL
    (
		userRolesList,
        parentMenuList,
        cssClassList,
        routeLinkList,
        routeLinkClassList,
        iconList,
        dropdownIconList,
		nextMenuSlNo
    )
    SELECT @URJson,
		   @PMJson,
           @CCJson,
           @RLJson,
           @RLCJson,
           @IconJson,
           @DDIJson,
		   @nextMenuSlNo

    SET @result =
    (
        SELECT *
        FROM #AppUserRoleMenuInitTBL
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DROP TABLE #AppUserRoleMenuInitTBL;

    SELECT @result AS result;
 
END;

GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 4/20/2025 8:49:07 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUserProfile]    Script Date: 4/20/2025 8:49:07 PM ******/
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
USE [master]
GO
ALTER DATABASE [SecurityDB] SET  READ_WRITE 
GO
