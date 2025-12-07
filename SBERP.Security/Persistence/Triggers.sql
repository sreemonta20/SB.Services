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