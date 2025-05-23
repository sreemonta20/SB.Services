USE [SecurityDB]
GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserMenus]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserMenusLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserProfiles]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserProfilesLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserRoleMenus]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserRoleMenusLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserRoles]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUserRolesLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUsers]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[AppUsersLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  Table [dbo].[SecurityLog]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserMenu]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserRoleMenu]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserMenu]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserProfile]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusByUserId]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusPagingWithSearch]    Script Date: 6/9/2024 3:58:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user menu list using paging with search
-- =============================================
--EXEC SP_GetAllAppUserMenusPagingWithSearch '','','',1,10
--EXEC SP_GetAllAppUserMenusPagingWithSearch 'nav-icon fas fa-cog','Icon','ASC',1,2
--EXEC SP_GetAllAppUserMenusPagingWithSearch 1,5,'User','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearch 1,5,'','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearch 2,5,'','','ASC'
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
                WHEN am.[ParentId] IS NOT NULL THEN pm.[Name]
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
                    CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'ASC' THEN am.[Name] END ASC,
                    CASE WHEN @SortColumnName = 'Name' AND @SortColumnDirection = 'DESC' THEN am.[Name] END DESC,
                    CASE WHEN @SortColumnName = 'SerialNo' AND @SortColumnDirection = 'ASC' THEN am.[SerialNo] END ASC,
                    CASE WHEN @SortColumnName = 'SerialNo' AND @SortColumnDirection = 'DESC' THEN am.[SerialNo] END DESC,
                    CASE WHEN @SortColumnName = '' THEN am.[SerialNo] END ASC -- Default sorting
            ) AS RowNum
        FROM 
            [dbo].[AppUserMenus] am
        LEFT JOIN 
            [dbo].[AppUserMenus] pm ON am.[ParentId] = pm.[Id]
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
            [ParentName], -- Include ParentName here
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusPagingWithSearchTest]    Script Date: 7/3/2024 10:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user menu list using paging with search
-- =============================================
--EXEC SP_GetAllAppUserMenusPagingWithSearchTest '','','',1,10
--EXEC SP_GetAllAppUserMenusPagingWithSearchTest 'nav-icon fas fa-cog','Icon','ASC',1,2
--EXEC SP_GetAllAppUserMenusPagingWithSearchTest 1,5,'User','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearchTest 1,5,'','','ASC'
--EXEC SP_GetAllAppUserMenusPagingWithSearchTest 2,5,'','','ASC'
CREATE PROCEDURE [dbo].[SP_GetAllAppUserMenusPagingWithSearchTest]
@SearchTerm AS VARCHAR(50)='',
@SortColumnName AS VARCHAR(50)='',
@SortColumnDirection AS VARCHAR(50)='',
@PageIndex AS INT=0,
@PageSize AS INT=10
AS
BEGIN
	DECLARE @QUERY AS VARCHAR(MAX)='',@ORDER_QUERY AS VARCHAR(MAX)='',@CONDITIONS AS VARCHAR(MAX)='',
	@PAGINATION AS VARCHAR(MAX)=''

	SET @QUERY='SELECT AUM.Id, AUM.Name,AUM.IsHeader,AUM.IsModule,AUM.IsComponent,AUM.CssClass,AUM.RouteLink,AUM.RouteLinkClass,AUM.Icon,AUM.Remark,
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfileList]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfiles]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfilesPagingSearch]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserRoleMenusPagingWithSearch]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserMenuHierarchyByMenuId]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserProfileById]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserRoleMenuInitialData]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 6/9/2024 3:58:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUserProfile]    Script Date: 6/9/2024 3:58:13 PM ******/
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

------------------------------------------AppUserRole Tabale Data (27.06.2024 3.31 PM) Starts-----------------------------------------------------------------
GO
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'7d95a0f7-24b6-4a92-b27d-0da53b31398e', N'Hardware', N'Hardware', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'8277faf6-d47d-4c87-94f7-46d1070d5bb3', N'Production', N'Production', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'78dc2158-8c43-4864-b3de-4ae4ea6e09cd', N'AI', N'AI', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'30e68fc8-92c3-4fec-bf0d-6a5df0192721', N'SourceAdmin', N'SourceAdmin', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'0dfd16d4-4e39-4d29-b44b-8d614e20452a', N'Civil', N'Civil', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'b0839373-15ee-41d2-a44e-9408878d1816', N'Mechanical', N'Mechanical', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'959fa738-2321-496f-b43c-c09b4e5c4116', N'HR', N'HR', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'0333b940-3ba7-41ff-9955-d0b919162346', N'ITAdmin', N'ITAdmin', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'404daac7-71b7-417b-bf75-d63658813ed4', N'ITHelpDesk', N'ITHelpDesk', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'1eb822e2-f6e5-4026-a6db-ef262af3f5da', N'QA', N'QA', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'd8c5ba8e-58ca-40ce-a3b2-f01cef593caa', N'SecurityAdmin', N'SecurityAdmin', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
INSERT [dbo].[AppUserRoles] ([Id], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive]) VALUES (N'4502575b-84a9-4fd1-8784-f7e47e16cca2', N'Complience', N'Complience', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1)
GO
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'3c267d64-9ce7-4da7-a323-03ae149a8787', N'1eb822e2-f6e5-4026-a6db-ef262af3f5da', N'QA', N'QA', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'2e00788d-9d3f-48bb-a8b3-157760b836e7', N'404daac7-71b7-417b-bf75-d63658813ed4', N'ITHelpDesk', N'ITHelpDesk', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Anannya Rohine', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'5f15053c-3d3a-4fc7-834a-48dd73e8fd9e', N'0dfd16d4-4e39-4d29-b44b-8d614e20452a', N'Civil', N'Civil', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'c8ca99bb-52cf-4a59-bf78-5524b6e07bc8', N'78dc2158-8c43-4864-b3de-4ae4ea6e09cd', N'AI', N'AI', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'e520008d-2f18-4773-83b5-8c5c360938d6', N'b0839373-15ee-41d2-a44e-9408878d1816', N'Mechanical', N'Mechanical', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'8615da0d-4e2f-4f0c-8750-97d947fdfb2a', N'8277faf6-d47d-4c87-94f7-46d1070d5bb3', N'Production', N'Production', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Anannya Rohine', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'f89a4909-b044-4a3b-80d2-d48a48f17694', N'd8c5ba8e-58ca-40ce-a3b2-f01cef593caa', N'SecurityAdmin', N'SecurityAdmin', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'58af571b-962c-4081-9174-db983febe8f7', N'4502575b-84a9-4fd1-8784-f7e47e16cca2', N'Complience', N'Complience', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'bd78e487-f239-4c4a-8841-dcd2b8fc26b7', N'd8c5ba8e-58ca-40ce-a3b2-f01cef593caa', N'SecurityAdmin', N'SecurityAdmin', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Updated')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'270fa767-0dad-4f5d-944c-e406a802d358', N'7d95a0f7-24b6-4a92-b27d-0da53b31398e', N'Hardware', N'Hardware', N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:32:14.573' AS DateTime), N'C047D662-9F0E-4358-B323-15EC3081312C', CAST(N'2024-06-09T11:35:48.737' AS DateTime), 1, N'Sreemonta Bhowmik', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'77c6b013-6f38-4eac-89a2-ed5e1d35e639', N'30e68fc8-92c3-4fec-bf0d-6a5df0192721', N'SourceAdmin', N'SourceAdmin', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Anannya Rohine', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'c5a8d0e9-a3f9-4b46-a1cc-f69289b88682', N'959fa738-2321-496f-b43c-c09b4e5c4116', N'HR', N'HR', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Anannya Rohine', N'Created')
INSERT [dbo].[AppUserRolesLog] ([Id], [AppUserRoleId], [RoleName], [Description], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [PerformedUser], [Action]) VALUES (N'60d41d5e-717b-4d16-ba98-fbf42a971d89', N'0333b940-3ba7-41ff-9955-d0b919162346', N'ITAdmin', N'ITAdmin', N'EFEDC118-3459-4C2E-9158-AD69196A59E0', CAST(N'2024-06-09T11:32:14.573' AS DateTime), NULL, NULL, 1, N'Anannya Rohine', N'Created')
GO
------------------------------------------AppUserRole Tabale Data (27.06.2024 3.31 PM) Ends-----------------------------------------------------------------