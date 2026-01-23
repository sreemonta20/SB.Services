GO
/****** Object:  Trigger [dbo].[TRG_UpdateAppUserMenus]    Script Date: 1/23/2026 5:00:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_UpdateAppUserMenus]
ON [dbo].[AppUserMenus]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],[Icon],[Remark],
		[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],Tbl.[Icon],
        Tbl.[Remark],Tbl.[ParentId],Tbl.[DropdownIcon],Tbl.[SerialNo],Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],
        (SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,'Updated' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END

GO
/****** Object:  Trigger [dbo].[TRG_InsertAppUserMenus]    Script Date: 1/23/2026 5:00:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_InsertAppUserMenus]
ON [dbo].[AppUserMenus]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],
        [Icon],[Remark],[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],
        Tbl.[Icon],Tbl.[Remark],Tbl.[ParentId],Tbl.[DropdownIcon],Tbl.[SerialNo],Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],
        Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[CreatedBy]) AS PerformedUser,'Created' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END

GO
/****** Object:  Trigger [dbo].[TRG_DeleteAppUserMenus]    Script Date: 1/23/2026 4:59:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_DeleteAppUserMenus]
ON [dbo].[AppUserMenus]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],[Icon],
        [Remark],[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],Tbl.[Icon],
        Tbl.[Remark],Tbl.[ParentId],Tbl.[DropdownIcon],Tbl.[SerialNo],Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],
        (SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,'Deleted' AS [Action]
        FROM DELETED Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END

GO
/****** Object:  Trigger [dbo].[TRG_UpdateAppUserProfiles]    Script Date: 1/23/2026 5:02:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_UpdateAppUserProfiles]
ON [dbo].[AppUserProfiles]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[CreatedBy],Tbl.[CreatedDate],
		Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,'Updated' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END

GO
/****** Object:  Trigger [dbo].[TRG_InsertAppUserProfiles]    Script Date: 1/23/2026 5:02:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_InsertAppUserProfiles]
ON [dbo].[AppUserProfiles]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[CreatedBy],Tbl.[CreatedDate],
		Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[CreatedBy]) AS PerformedUser,'Created' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_DeleteAppUserProfiles]    Script Date: 1/23/2026 5:02:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_DeleteAppUserProfiles]
ON [dbo].[AppUserProfiles]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[CreatedBy],Tbl.[CreatedDate],
		Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,'Deleted' AS [Action]
        FROM DELETED Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_UpdateAppUserRoleMenus]    Script Date: 1/23/2026 5:04:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_UpdateAppUserRoleMenus]
ON [dbo].[AppUserRoleMenus]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRoleMenusLog ([Id],[AppUserRoleMenuId],[AppUserRoleId],[AppUserMenuId],[IsView],[IsCreate],[IsUpdate],[IsDelete],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserRoleId],Tbl.[AppUserMenuId],Tbl.[IsView],Tbl.[IsCreate],Tbl.[IsUpdate],Tbl.[IsDelete],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Updated' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_InsertAppUserRoleMenus]    Script Date: 1/23/2026 5:04:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_InsertAppUserRoleMenus]
ON [dbo].[AppUserRoleMenus]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRoleMenusLog ([Id],[AppUserRoleMenuId],[AppUserRoleId],[AppUserMenuId],[IsView],[IsCreate],[IsUpdate],[IsDelete],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserRoleId],Tbl.[AppUserMenuId],Tbl.[IsView],Tbl.[IsCreate],Tbl.[IsUpdate],Tbl.[IsDelete],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[CreatedBy]) AS PerformedUser,
		'Created' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_DeleteAppUserRoleMenus]    Script Date: 1/23/2026 5:04:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_DeleteAppUserRoleMenus]
ON [dbo].[AppUserRoleMenus]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRoleMenusLog ([Id],[AppUserRoleMenuId],[AppUserRoleId],[AppUserMenuId],[IsView],[IsCreate],[IsUpdate],[IsDelete],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserRoleId],Tbl.[AppUserMenuId],Tbl.[IsView],Tbl.[IsCreate],Tbl.[IsUpdate],Tbl.[IsDelete],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Deleted' AS [Action]
        FROM DELETED Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_UpdateAppUserRoles]    Script Date: 1/23/2026 5:05:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_UpdateAppUserRoles]
ON [dbo].[AppUserRoles]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRolesLog ([Id],[AppUserRoleId],[RoleName],[Description],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[RoleName],Tbl.[Description],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Updated' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_InsertAppUserRoles]    Script Date: 1/23/2026 5:05:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_InsertAppUserRoles]
ON [dbo].[AppUserRoles]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRolesLog ([Id],[AppUserRoleId],[RoleName],[Description],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[RoleName],Tbl.[Description],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[CreatedBy]) AS PerformedUser,
		'Created' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_DeleteAppUserRoles]    Script Date: 1/23/2026 5:05:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_DeleteAppUserRoles]
ON [dbo].[AppUserRoles]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUserRolesLog ([Id],[AppUserRoleId],[RoleName],[Description],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[RoleName],Tbl.[Description],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Deleted' AS [Action]
        FROM DELETED Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_UpdateAppUsers]    Script Date: 1/23/2026 5:06:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_UpdateAppUsers]
ON [dbo].[AppUsers]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],[RefreshToken],[RefreshTokenExpiryTime],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],Tbl.[RefreshToken],Tbl.[RefreshTokenExpiryTime],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Updated' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_InsertAppUsers]    Script Date: 1/23/2026 5:06:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_InsertAppUsers]
ON [dbo].[AppUsers]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],[RefreshToken],[RefreshTokenExpiryTime],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],Tbl.[RefreshToken],Tbl.[RefreshTokenExpiryTime],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[CreatedBy]) AS PerformedUser,
		'Created' AS [Action]
        FROM inserted Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
/****** Object:  Trigger [dbo].[TRG_DeleteAppUsers]    Script Date: 1/23/2026 5:06:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRG_DeleteAppUsers]
ON [dbo].[AppUsers]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],[RefreshToken],[RefreshTokenExpiryTime],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],Tbl.[RefreshToken],Tbl.[RefreshTokenExpiryTime],
		Tbl.[CreatedBy],Tbl.[CreatedDate],Tbl.[UpdatedBy],Tbl.[UpdatedDate],Tbl.[IsActive],(SELECT FullName FROM AppUserProfiles WHERE Id = Tbl.[UpdatedBy]) AS PerformedUser,
		'Deleted' AS [Action]
        FROM DELETED Tbl;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
        ROLLBACK TRANSACTION;
    END CATCH
END
GO