-- =====================================================================================================
-- SecurityDB — all SQL objects (functions, stored procedures, triggers)
-- -----------------------------------------------------------------------------------------------------
-- The only SQL file of SBERP.Security. Tables, keys, indexes and seed data are not here: they come from the
-- EF Core model (SecurityDBContext, HasData). This file is an Embedded Resource and is applied by the newest
-- migration through SqlScriptHelper.ApplyAll, after the tables exist:
--     dotnet ef database update      (fresh database at a customer, or an upgrade)
-- Every object is dropped if it exists and created again, so the file can run any number of times.
-- SQL Server 2016 RTM compatible: no CREATE OR ALTER, no STRING_AGG.
--
-- Sections (in run order):
--    1. Functions
--    2. StoredProcedures
--    3. Triggers
-- =====================================================================================================
GO


-- #####################################################################################################
-- SECTION 1: Functions   (was 01_Functions.sql)
-- #####################################################################################################
-- =====================================================================================
-- SecurityDB SQL objects (functions).
-- Canonical copy: applied by EF migrations through SqlScriptHelper.ApplyAll
-- (Persistence/Sql/*.sql, Embedded Resource, file-name order). Re-runnable:
-- every object is dropped (DROP ... IF EXISTS) and re-created. SQL Server 2016 RTM
-- has no CREATE OR ALTER / STRING_AGG, so do not use them here.
-- =====================================================================================
DROP FUNCTION IF EXISTS [dbo].[GetChildMenus];
GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 1/23/2026 4:58:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 14.07.2023
-- Description: To generate children menu under particular menu
-- Change:      25.10.2026 - IsSubModule level, ordered by SerialNo, inactive menus hidden.
--              The recursion keeps the depth open-ended:
--              Header > Module > (Sub Module >)* Component.
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
			,AUM.[IsSubModule]
			,AUM.[IsComponent]
			,AUM.[CssClass]
			,AUM.[IsRouteLink]
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
            AUM.ParentId = @parentId AND AURM.AppUserRoleId = @RoleId AND AURM.IsActive = 1 AND AUM.IsActive = 1
        ORDER BY AUM.[SerialNo]
        FOR JSON PATH
    )
END
GO
GO
GO

-- #####################################################################################################
-- SECTION 2: StoredProcedures   (was 02_StoredProcedures.sql)
-- #####################################################################################################
-- =====================================================================================
-- SecurityDB SQL objects (stored procedures).
-- Canonical copy: applied by EF migrations through SqlScriptHelper.ApplyAll
-- (Persistence/Sql/*.sql, Embedded Resource, file-name order). Re-runnable:
-- every object is dropped (DROP ... IF EXISTS) and re-created. SQL Server 2016 RTM
-- has no CREATE OR ALTER / STRING_AGG, so do not use them here.
-- =====================================================================================
DROP PROCEDURE IF EXISTS [dbo].[SP_CreateUpdateAppUserMenu];
GO
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserMenu]    Script Date: 1/23/2026 4:35:01 AM ******/
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
	@IsRouteLink		BIT,
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
	@IsActive			BIT,
	@IsSubModule		BIT = 0 -- last, so positional callers written before it keep working
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
			INSERT INTO AppUserMenus (Id, Name, IsHeader, IsModule, IsSubModule, IsComponent, CssClass, IsRouteLink, RouteLink, RouteLinkClass, Icon, Remark, ParentId, DropdownIcon, SerialNo, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive)
			VALUES (@Id, @Name, @IsHeader, @IsModule, ISNULL(@IsSubModule, 0), @IsComponent, @CssClass, @IsRouteLink, @RouteLink, @RouteLinkClass, @Icon, @Remark, @ParentId, @DropdownIcon, @SerialNo, @CreatedBy, GETUTCDATE(), NULL, NULL, @IsActive);

			SELECT @@ROWCOUNT AS 'RowsAffected';
		END

    END
	ELSE IF @ActionName = 'Update' -- Update
    BEGIN
		IF EXISTS (SELECT 1 FROM AppUserMenus WHERE Id = @Id)
		BEGIN
			UPDATE AppUserMenus SET Name = @Name, IsHeader = @IsHeader,  IsModule = @IsModule, IsSubModule = ISNULL(@IsSubModule, 0), IsComponent = @IsComponent, CssClass = @CssClass, IsRouteLink = @IsRouteLink, RouteLink = @RouteLink,
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

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_DeleteAppUserMenu];
GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserMenu]    Script Date: 1/23/2026 4:37:21 AM ******/
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
    -- A header / module / sub module that still has active child menus would orphan them
    IF EXISTS (SELECT 1 FROM AppUserMenus WHERE ParentId = @ConvertedMenuId AND IsActive = 1)
    BEGIN
        SET @Result = N'{"message": "This menu has active child menus. Move or remove them first", "rowcount": 0, "sucess":"false"}';
        SELECT @Result
        RETURN
    END
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
DROP PROCEDURE IF EXISTS [dbo].[SP_DeleteAppUserProfile];
GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserProfile]    Script Date: 1/23/2026 4:40:40 AM ******/
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
    @Id         UNIQUEIDENTIFIER, -- AppUserProfile.Id
    @IsDelete   BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RowsAffected INT = 0;

    BEGIN TRY
        BEGIN TRAN;

        /* ---------- Handle AppUsers ---------- */
        IF EXISTS (SELECT 1 FROM dbo.AppUsers WHERE AppUserProfileId = @Id)
        BEGIN
            IF (@IsDelete = 1)
            BEGIN
                DELETE FROM dbo.AppUsers
                WHERE AppUserProfileId = @Id;

                SET @RowsAffected += @@ROWCOUNT;
            END
            ELSE
            BEGIN
                UPDATE dbo.AppUsers
                SET IsActive = 0
                WHERE AppUserProfileId = @Id;

                SET @RowsAffected += @@ROWCOUNT;
            END
        END

        /* ---------- Handle AppUserProfiles ---------- */
        IF (@IsDelete = 1)
        BEGIN
            DELETE FROM dbo.AppUserProfiles
            WHERE Id = @Id;

            SET @RowsAffected += @@ROWCOUNT;
        END
        ELSE
        BEGIN
            UPDATE dbo.AppUserProfiles
            SET IsActive = 0
            WHERE Id = @Id;

            SET @RowsAffected += @@ROWCOUNT;
        END

        COMMIT TRAN;

        SELECT @RowsAffected AS RowsAffected;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_GetAllAppUserMenusByUserId];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusByUserId]    Script Date: 1/23/2026 4:42:10 AM ******/
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
		,AUM.[IsSubModule]
		,AUM.[IsComponent]
		,AUM.[CssClass]
		,AUM.[IsRouteLink]
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
		WHERE AURM.AppUserRoleId = @RoleId AND AUM.ParentId IS NULL AND AURM.IsActive = 1 AND AUM.IsActive = 1

	ORDER BY AUM.SerialNo
        --FOR JSON AUTO,INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER
		--FOR JSON AUTO,INCLUDE_NULL_VALUES, ROOT('Menu')
		FOR JSON PATH,INCLUDE_NULL_VALUES
    );

    SELECT @JsonMenu AS JsonMenu;
END

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_GetAllAppUserMenusPagingWithSearch];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusPagingWithSearch]    Script Date: 1/23/2026 4:48:09 AM ******/
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
    DECLARE @Result NVARCHAR(MAX);
    DECLARE @Items NVARCHAR(MAX);

    CREATE TABLE #AppUserMenuTBL
    (
        [RowCount] INT,
        [CurrentPage] INT,
        [PageSize] INT,
        [PageCount] INT,
        [Items] NVARCHAR(MAX)
    );

    -- Calculate total records with the search term filter
    SELECT @TotalRecords = COUNT(*)
    FROM [dbo].[AppUserMenus]
    WHERE
		  --IsActive = 1
    --      AND
		      (
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
            am.[IsSubModule],
            am.[IsComponent],
            am.[CssClass],
			am.[IsRouteLink],
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
					CASE WHEN @SortColumnName = 'IsSubModule' AND @SortColumnDirection = 'asc' THEN am.[IsSubModule] END ASC,
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
					CASE WHEN @SortColumnName = 'IsSubModule' AND @SortColumnDirection = 'desc' THEN am.[IsSubModule] END DESC,
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
            --am.IsActive = 1
            --AND
			(
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
            [IsSubModule],
            [IsComponent],
            [CssClass],
			[IsRouteLink],
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
DROP PROCEDURE IF EXISTS [dbo].[SP_GetAllAppUserProfilesPagingWithSearch];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfilesPagingWithSearch]    Script Date: 1/23/2026 4:50:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllAppUserProfilesPagingWithSearch 1,10,'','FullName','asc'
CREATE PROCEDURE [dbo].[SP_GetAllAppUserProfilesPagingWithSearch]
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
    DECLARE @Result NVARCHAR(MAX);
    DECLARE @Items NVARCHAR(MAX);

    CREATE TABLE #AppUserProfileTBL
    (
        [RowCount] INT,
        [CurrentPage] INT,
        [PageSize] INT,
        [PageCount] INT,
        [Items] NVARCHAR(MAX)
    );

    -- Calculate total records with the search term filter
    SELECT @TotalRecords = COUNT(*)
    FROM [dbo].[AppUserProfiles] AUP
	LEFT JOIN [dbo].[AppUserRoles] AUR ON AUR.Id = AUP.AppUserRoleId
	LEFT JOIN [dbo].[AppUsers] AU ON AU.AppUserProfileId = AUP.Id
    WHERE AUR.IsActive = 1
          AND (
                  @SearchTerm = ''
                  OR AUP.[FullName] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[Email] LIKE '%' + @SearchTerm + '%'
				  OR AUR.[RoleName] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[JobTitle] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[Address] LIKE '%' + @SearchTerm + '%'
				  OR AU.[UserName] LIKE '%' + @SearchTerm + '%'
              );

    -- Calculate total pages
    SET @TotalPages = CEILING(CAST(@TotalRecords AS FLOAT) / @PageSize);
    WITH SortedAppUserProfiles AS
    (
        SELECT
            AUP.[Id],
            AUP.[FullName],
            AUP.[Address],
            AUP.[Email],
            AUP.[AppUserRoleId],
			AUR.[RoleName],
			AU.[UserName],
            AUP.[JobTitle],
            AUP.[EmployeeId],
            AUP.[ProfilePhotoId],
            AUP.[CreatedBy],
			AUP.[CreatedDate],
            AUP.[UpdatedBy],
            AUP.[UpdatedDate],
            AUP.[IsActive],
            ROW_NUMBER() OVER
            (
                ORDER BY
					CASE WHEN @SortColumnName = 'FullName' AND @SortColumnDirection = 'asc' THEN AUP.[FullName] END ASC,
					CASE WHEN @SortColumnName = 'Address' AND @SortColumnDirection = 'asc' THEN AUP.[Address] END ASC,
					CASE WHEN @SortColumnName = 'Email' AND @SortColumnDirection = 'asc' THEN AUP.[Email] END ASC,
					CASE WHEN @SortColumnName = 'RoleName' AND @SortColumnDirection = 'asc' THEN AUR.[RoleName] END ASC,
					CASE WHEN @SortColumnName = 'JobTitle' AND @SortColumnDirection = 'asc' THEN AUP.[JobTitle] END ASC,
					CASE WHEN @SortColumnName = 'UserName' AND @SortColumnDirection = 'asc' THEN AU.[UserName] END ASC,
					CASE WHEN @SortColumnName = 'IsActive' AND @SortColumnDirection = 'asc' THEN AUP.[IsActive] END ASC,

                    CASE WHEN @SortColumnName = 'FullName' AND @SortColumnDirection = 'desc' THEN AUP.[FullName] END DESC,
					CASE WHEN @SortColumnName = 'Address' AND @SortColumnDirection = 'desc' THEN AUP.[Address] END DESC,
					CASE WHEN @SortColumnName = 'Email' AND @SortColumnDirection = 'desc' THEN AUP.[Email] END DESC,
					CASE WHEN @SortColumnName = 'RoleName' AND @SortColumnDirection = 'desc' THEN AUR.[RoleName] END DESC,
					CASE WHEN @SortColumnName = 'JobTitle' AND @SortColumnDirection = 'desc' THEN AUP.[JobTitle] END DESC,
					CASE WHEN @SortColumnName = 'UserName' AND @SortColumnDirection = 'desc' THEN AU.[UserName] END DESC,
					CASE WHEN @SortColumnName = 'IsActive' AND @SortColumnDirection = 'desc' THEN AUP.[IsActive] END DESC
            ) AS RowNum
        FROM
            [dbo].[AppUserProfiles] AUP
	LEFT JOIN [dbo].[AppUserRoles] AUR ON AUR.Id = AUP.AppUserRoleId
	LEFT JOIN [dbo].[AppUsers] AU ON AU.AppUserProfileId = AUP.Id
    WHERE AUR.IsActive = 1
          AND (
                  @SearchTerm = ''
                  OR AUP.[FullName] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[Email] LIKE '%' + @SearchTerm + '%'
				  OR AUR.[RoleName] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[JobTitle] LIKE '%' + @SearchTerm + '%'
				  OR AUP.[Address] LIKE '%' + @SearchTerm + '%'
				  OR AU.[UserName] LIKE '%' + @SearchTerm + '%'
              )
    )
    SELECT @Items =
    (
        SELECT
            [Id],
            [FullName],
            [Address],
            [Email],
            [AppUserRoleId],
			[RoleName],
			[UserName],
            [JobTitle],
            [EmployeeId],
            [ProfilePhotoId],
            [CreatedBy],
			[CreatedDate],
            [UpdatedBy],
            [UpdatedDate],
            [IsActive]
        FROM
            SortedAppUserProfiles
        WHERE
            RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #AppUserProfileTBL
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
        SELECT * FROM #AppUserProfileTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DROP TABLE #AppUserProfileTBL;

    SELECT @Result AS result;
END;

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_GetAppUserProfileById];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserProfileById]    Script Date: 1/23/2026 4:51:27 AM ******/
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
	SELECT AUP.Id, AUP.FullName,AUP.Address,AUP.Email,AUP.AppUserRoleId, AUR.RoleName, AU.UserName,
	AUP.JobTitle, AUP.EmployeeId, AUP.ProfilePhotoId,
	AUP.CreatedBy,AUP.CreatedDate,AUP.UpdatedBy,AUP.UpdatedDate,AUP.IsActive
	FROM AppUserProfiles AUP
	LEFT JOIN AppUserRoles AUR ON AUR.Id = AUP.AppUserRoleId
	LEFT JOIN AppUsers AU ON AU.AppUserProfileId = AUP.Id
	WHERE AUP.Id = @Id
END

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_GetAppUserRoleMenuInitialData];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserRoleMenuInitialData]    Script Date: 1/23/2026 4:52:57 AM ******/
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
		userRolesList		NVARCHAR(MAX),
        parentMenuList		NVARCHAR(MAX),
        cssClassList		NVARCHAR(MAX),
        routeLinkList		NVARCHAR(MAX),
        routeLinkClassList	NVARCHAR(MAX),
        iconList			NVARCHAR(MAX),
        dropdownIconList	NVARCHAR(MAX),
		nextMenuSlNo		INT
    );

    DECLARE @URJson			NVARCHAR(MAX),
			@PMJson			NVARCHAR(MAX),
            @CCJson			NVARCHAR(MAX),
            @RLJson			NVARCHAR(MAX),
            @RLCJson		NVARCHAR(MAX),
            @IconJson		NVARCHAR(MAX),
            @DDIJson		NVARCHAR(MAX),
            @result			NVARCHAR(MAX),
			@nextMenuSlNo	INT;

	SET @URJson =
    (
        SELECT DISTINCT [Id] id,
               [RoleName] name
        FROM AppUserRoles
        WHERE [RoleName] IS NOT NULL
        FOR JSON AUTO
    );

    -- Only Header / Module / Sub Module rows can own children. "type" lets the UI offer
    -- just the valid parents for the menu type being edited.
    SET @PMJson =
    (
        SELECT [Id] id,
               [Name] + CASE
                            WHEN [IsHeader] = 1    THEN ' (Header Menu)'
                            WHEN [IsModule] = 1    THEN ' (Module Menu)'
                            WHEN [IsSubModule] = 1 THEN ' (Sub Module Menu)'
                            ELSE ' (Business Menu)'
                        END AS name,
               CASE
                    WHEN [IsHeader] = 1    THEN 'Header'
                    WHEN [IsModule] = 1    THEN 'Module'
                    WHEN [IsSubModule] = 1 THEN 'SubModule'
                    ELSE 'Component'
               END AS type
        FROM AppUserMenus
        WHERE [Name] IS NOT NULL
          AND ISNULL([IsComponent], 0) = 0
        ORDER BY [SerialNo]
        FOR JSON PATH
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

	SET @nextMenuSlNo = (SELECT ISNULL(MAX(SerialNo), 0) + 1 AS SerialNo FROM AppUserMenus)

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
DROP PROCEDURE IF EXISTS [dbo].[SP_GetRoleMenusPagingWithSearch];
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoleMenusPagingWithSearch]    Script Date: 1/23/2026 4:54:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 06.12.2025
-- Description:	This SP is used to fetch all the menus belong or not belong to a specific role
-- =============================================
--EXEC SP_GetRoleMenusPagingWithSearch '1B15CE5A-56B3-4EB9-8286-6E27F770B0DA',1,20,'','MenuName','asc'
CREATE PROCEDURE [dbo].[SP_GetRoleMenusPagingWithSearch]
(
    @AppUserRoleId UNIQUEIDENTIFIER,
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm VARCHAR(50) = '',
    @SortColumnName VARCHAR(50) = '',
    @SortColumnDirection VARCHAR(50) = ''
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRecords INT;
    DECLARE @TotalPages INT;
    DECLARE @Result NVARCHAR(MAX);
    DECLARE @Items NVARCHAR(MAX);

    CREATE TABLE #RoleMenuTBL
    (
        [RowCount] INT,
        [CurrentPage] INT,
        [PageSize] INT,
        [PageCount] INT,
        [Items] NVARCHAR(MAX)
    );

    -- Count total menus (joined with role menu) filtered by role + search term
    SELECT @TotalRecords = COUNT(*)
    FROM [dbo].[AppUserMenus] M
    LEFT JOIN [dbo].[AppUserRoleMenus] RM
        ON RM.AppUserMenuId = M.Id
        AND RM.AppUserRoleId = @AppUserRoleId
    WHERE M.IsActive = 1
      AND (
            @SearchTerm = ''
            OR M.[Name] LIKE '%' + @SearchTerm + '%'
          );

    SET @TotalPages = CEILING(CAST(@TotalRecords AS FLOAT) / @PageSize);

    ;WITH SortedRoleMenus AS
    (
        SELECT
            M.[Id]								AS MenuId,
            M.[Name]							AS MenuName,
            M.[IsHeader]						AS IsHeader,
            M.[IsModule]						AS IsModule,
            M.[IsSubModule]						AS IsSubModule,
            M.[IsComponent]						AS IsComponent,
            M.[IsRouteLink]						AS IsRouteLink,
            M.[RouteLink]						AS Url,
            M.[SerialNo],
            M.[IsActive]						AS IsActiveMenu,
            ISNULL(RM.[Id], NULL)				AS RoleMenuId,
            @AppUserRoleId						AS RoleId,
            ISNULL(RM.[IsView],   0)			AS IsView,
            ISNULL(RM.[IsCreate], 0)			AS IsCreate,
            ISNULL(RM.[IsUpdate], 0)			AS IsUpdate,
            ISNULL(RM.[IsDelete], 0)			AS IsDelete,
            ISNULL(RM.[IsActive], 0)			AS IsActive,
            ROW_NUMBER() OVER
            (
                ORDER BY
                    CASE WHEN @SortColumnName = 'MenuName'    AND @SortColumnDirection = 'asc'  THEN M.[Name]     END ASC,
                    CASE WHEN @SortColumnName = 'IsHeader'    AND @SortColumnDirection = 'asc'  THEN M.[IsHeader] END ASC,
                    CASE WHEN @SortColumnName = 'IsModule'    AND @SortColumnDirection = 'asc'  THEN M.[IsModule] END ASC,
                    CASE WHEN @SortColumnName = 'IsComponent' AND @SortColumnDirection = 'asc'  THEN M.[IsComponent] END ASC,
                    CASE WHEN @SortColumnName = 'Url'         AND @SortColumnDirection = 'asc'  THEN M.[RouteLink] END ASC,
                    CASE WHEN @SortColumnName = 'IsActive'    AND @SortColumnDirection = 'asc'  THEN ISNULL(RM.[IsActive],0) END ASC,
                    CASE WHEN @SortColumnName = 'SerialNo'    AND @SortColumnDirection = 'asc'  THEN M.[SerialNo] END ASC,

                    CASE WHEN @SortColumnName = 'MenuName'    AND @SortColumnDirection = 'desc' THEN M.[Name]     END DESC,
                    CASE WHEN @SortColumnName = 'IsHeader'    AND @SortColumnDirection = 'desc' THEN M.[IsHeader] END DESC,
                    CASE WHEN @SortColumnName = 'IsModule'    AND @SortColumnDirection = 'desc' THEN M.[IsModule] END DESC,
                    CASE WHEN @SortColumnName = 'IsComponent' AND @SortColumnDirection = 'desc' THEN M.[IsComponent] END DESC,
                    CASE WHEN @SortColumnName = 'Url'         AND @SortColumnDirection = 'desc' THEN M.[RouteLink] END DESC,
                    CASE WHEN @SortColumnName = 'IsActive'    AND @SortColumnDirection = 'desc' THEN ISNULL(RM.[IsActive],0) END DESC,
                    CASE WHEN @SortColumnName = 'SerialNo'    AND @SortColumnDirection = 'desc' THEN M.[SerialNo] END DESC,

                    M.[SerialNo] ASC
            ) AS RowNum
        FROM [dbo].[AppUserMenus] M
        LEFT JOIN [dbo].[AppUserRoleMenus] RM
            ON RM.AppUserMenuId = M.Id
            AND RM.AppUserRoleId = @AppUserRoleId
        WHERE M.IsActive = 1
          AND (
                @SearchTerm = ''
                OR M.[Name] LIKE '%' + @SearchTerm + '%'
          )
    )
    SELECT @Items =
    (
        SELECT
            MenuId,
            MenuName,
            IsHeader,
            IsModule,
            IsSubModule,
            IsComponent,
            IsRouteLink,
            Url,
            SerialNo,
            IsActiveMenu,
            RoleMenuId,
            RoleId,
            IsView,
            IsCreate,
            IsUpdate,
            IsDelete,
            IsActive
        FROM SortedRoleMenus
        WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize
        FOR JSON AUTO
    );

    INSERT INTO #RoleMenuTBL
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
        SELECT * FROM #RoleMenuTBL FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DROP TABLE #RoleMenuTBL;

    SELECT @Result AS result;
END

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_SaveUpdateAppUser];
GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 1/23/2026 4:56:19 AM ******/
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
        INSERT INTO AppUsers ([Id],[AppUserProfileId],[UserName],[Password],[SaltKey],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive])
     VALUES
           (@Id,@AppUserProfileId, @UserName,@Password, @SaltKey, @CreatedBy,@CreatedDate,NULL, NULL, @IsActive)

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update' -- Update
    BEGIN
        UPDATE AppUsers SET [Id] = @Id,[AppUserProfileId] = @AppUserProfileId,[UserName] = @UserName,[Password] = @Password,[SaltKey] = @SaltKey,
		[UpdatedBy] = @UpdatedBy,[UpdatedDate] = @UpdatedDate, IsActive = @IsActive
        WHERE [Id] = @Id;

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE
    BEGIN
        RAISERROR('Invalid action flag. Must be either ''Save'' or ''Update''.', 16, 1);
    END
END

GO
DROP PROCEDURE IF EXISTS [dbo].[SP_SaveUpdateRoleMenuInBulk];
GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateRoleMenuInBulk]    Script Date: 1/23/2026 4:57:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 07.12.2025
-- Description:	This SP is used to create or update role menu permission for a specific role
-- Change:      25.10.2026 - a menu is only shown when its whole parent chain is granted
--              (Header > Module > Sub Module > Component), so every ancestor of an active
--              role menu is (re)activated without CRUD flags.
-- =============================================
CREATE PROCEDURE [dbo].[SP_SaveUpdateRoleMenuInBulk]
(
    @AppUserRoleId UNIQUEIDENTIFIER,
    @JsonData NVARCHAR(MAX),
    @UserId NVARCHAR(200)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Permissions TABLE
    (
        RoleMenuId UNIQUEIDENTIFIER NULL,
        MenuId UNIQUEIDENTIFIER,
        IsView BIT,
        IsCreate BIT,
        IsUpdate BIT,
        IsDelete BIT,
        IsActive BIT
    );

    INSERT INTO @Permissions (RoleMenuId, MenuId, IsView, IsCreate, IsUpdate, IsDelete, IsActive)
    SELECT
        JSON_VALUE(value, '$.RoleMenuId'),
        JSON_VALUE(value, '$.MenuId'),
        JSON_VALUE(value, '$.IsView'),
        JSON_VALUE(value, '$.IsCreate'),
        JSON_VALUE(value, '$.IsUpdate'),
        JSON_VALUE(value, '$.IsDelete'),
        JSON_VALUE(value, '$.IsActive')
    FROM OPENJSON(@JsonData);

    MERGE AppUserRoleMenus AS target
    USING @Permissions AS source
        ON target.Id = source.RoleMenuId

    WHEN MATCHED THEN
        UPDATE SET
            target.IsView = source.IsView,
            target.IsCreate = source.IsCreate,
            target.IsUpdate = source.IsUpdate,
            target.IsDelete = source.IsDelete,
            target.IsActive = source.IsActive,
            target.UpdatedBy = @UserId,
            target.UpdatedDate = GETUTCDATE()

    WHEN NOT MATCHED THEN
        INSERT (Id, AppUserRoleId, AppUserMenuId, IsView, IsCreate, IsUpdate, IsDelete, IsActive, CreatedBy, CreatedDate)
        VALUES (NEWID(), @AppUserRoleId, source.MenuId,
                source.IsView, source.IsCreate, source.IsUpdate, source.IsDelete, source.IsActive,
                @UserId, GETUTCDATE());

    -----------------------------------------------------
    -- Grant the ancestors of every active role menu
    -----------------------------------------------------
    ;WITH Ancestors AS
    (
        SELECT M.ParentId AS MenuId
        FROM AppUserRoleMenus RM
        INNER JOIN AppUserMenus M ON M.Id = RM.AppUserMenuId
        WHERE RM.AppUserRoleId = @AppUserRoleId AND RM.IsActive = 1 AND M.ParentId IS NOT NULL
        UNION ALL
        SELECT M.ParentId
        FROM Ancestors A
        INNER JOIN AppUserMenus M ON M.Id = A.MenuId
        WHERE M.ParentId IS NOT NULL
    )
    SELECT DISTINCT MenuId INTO #AncestorMenus FROM Ancestors OPTION (MAXRECURSION 32);

    UPDATE RM
       SET RM.IsActive = 1, RM.UpdatedBy = @UserId, RM.UpdatedDate = GETUTCDATE()
      FROM AppUserRoleMenus RM
     INNER JOIN #AncestorMenus A ON A.MenuId = RM.AppUserMenuId
     WHERE RM.AppUserRoleId = @AppUserRoleId AND ISNULL(RM.IsActive, 0) = 0;

    INSERT INTO AppUserRoleMenus (Id, AppUserRoleId, AppUserMenuId, IsView, IsCreate, IsUpdate, IsDelete, IsActive, CreatedBy, CreatedDate)
    SELECT NEWID(), @AppUserRoleId, A.MenuId, 0, 0, 0, 0, 1, @UserId, GETUTCDATE()
      FROM #AncestorMenus A
     WHERE NOT EXISTS (SELECT 1 FROM AppUserRoleMenus RM WHERE RM.AppUserRoleId = @AppUserRoleId AND RM.AppUserMenuId = A.MenuId);

    DROP TABLE #AncestorMenus;

    -----------------------------------------------------
    -- Return SUCCESS explicitly as scalar value
    -----------------------------------------------------
    SELECT 'SUCCESS' AS Result;
END
GO
GO

-- #####################################################################################################
-- SECTION 3: Triggers   (was 03_Triggers.sql)
-- #####################################################################################################
-- =====================================================================================
-- SecurityDB SQL objects (audit triggers).
-- Canonical copy: applied by EF migrations through SqlScriptHelper.ApplyAll
-- (Persistence/Sql/*.sql, Embedded Resource, file-name order). Re-runnable:
-- every object is dropped (DROP ... IF EXISTS) and re-created. SQL Server 2016 RTM
-- has no CREATE OR ALTER / STRING_AGG, so do not use them here.
-- =====================================================================================
DROP TRIGGER IF EXISTS [dbo].[TRG_UpdateAppUserMenus];
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
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsSubModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],[Icon],[Remark],
		[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsSubModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],Tbl.[Icon],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_InsertAppUserMenus];
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
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsSubModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],
        [Icon],[Remark],[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsSubModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_DeleteAppUserMenus];
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
        INSERT INTO dbo.AppUserMenusLog ([Id],[AppUserMenuId],[Name],[IsHeader],[IsModule],[IsSubModule],[IsComponent],[CssClass],[IsRouteLink],[RouteLink],[RouteLinkClass],[Icon],
        [Remark],[ParentId],[DropdownIcon],[SerialNo],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[Name],Tbl.[IsHeader],Tbl.[IsModule],Tbl.[IsSubModule],Tbl.[IsComponent],Tbl.[CssClass],Tbl.[IsRouteLink],Tbl.[RouteLink],Tbl.[RouteLinkClass],Tbl.[Icon],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_UpdateAppUserProfiles];
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
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[JobTitle],[ProfilePhotoId],[ProfilePhotoLocation],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[JobTitle],Tbl.[ProfilePhotoId],Tbl.[ProfilePhotoLocation],Tbl.[CreatedBy],Tbl.[CreatedDate],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_InsertAppUserProfiles];
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
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[JobTitle],[ProfilePhotoId],[ProfilePhotoLocation],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[JobTitle],Tbl.[ProfilePhotoId],Tbl.[ProfilePhotoLocation],Tbl.[CreatedBy],Tbl.[CreatedDate],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_DeleteAppUserProfiles];
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
        INSERT INTO dbo.AppUserProfilesLog ([Id],[AppUserProfileId],[FullName],[Address],[Email],[AppUserRoleId],[JobTitle],[ProfilePhotoId],[ProfilePhotoLocation],[CreatedBy],[CreatedDate],
		[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[FullName],Tbl.[Address],Tbl.[Email],Tbl.[AppUserRoleId],Tbl.[JobTitle],Tbl.[ProfilePhotoId],Tbl.[ProfilePhotoLocation],Tbl.[CreatedBy],Tbl.[CreatedDate],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_UpdateAppUserRoleMenus];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_InsertAppUserRoleMenus];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_DeleteAppUserRoleMenus];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_UpdateAppUserRoles];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_InsertAppUserRoles];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_DeleteAppUserRoles];
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
DROP TRIGGER IF EXISTS [dbo].[TRG_UpdateAppUsers];
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
    -- login lockout bookkeeping (FailedLoginAttempts / LockoutEnd) is not a change of the user: no log row
    IF NOT (UPDATE([AppUserProfileId]) OR UPDATE([UserName]) OR UPDATE([Password]) OR UPDATE([SaltKey])
            OR UPDATE([CreatedBy]) OR UPDATE([CreatedDate]) OR UPDATE([UpdatedBy]) OR UPDATE([UpdatedDate]) OR UPDATE([IsActive]))
        RETURN;
    BEGIN TRY
        DECLARE @ErrorMsg NVARCHAR(4000);

        -- Insert log entry
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_InsertAppUsers];
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
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],
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
DROP TRIGGER IF EXISTS [dbo].[TRG_DeleteAppUsers];
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
        INSERT INTO dbo.AppUsersLog ([Id],[AppUserId],[AppUserProfileId],[UserName],[Password],[SaltKey],
		[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive],[PerformedUser],[Action])
        -- Assuming a new uniqueidentifier for log entry
		SELECT NEWID(),Tbl.[Id],Tbl.[AppUserProfileId],Tbl.[UserName],Tbl.[Password],Tbl.[SaltKey],
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
GO
GO
