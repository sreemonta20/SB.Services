USE [SecurityDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusByUserId]    Script Date: 12/5/2025 11:37:25 PM ******/
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
		WHERE AURM.AppUserRoleId = @RoleId AND AUM.ParentId IS NULL AND AURM.IsActive = 1
        
	ORDER BY SerialNo
        --FOR JSON AUTO,INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER
		--FOR JSON AUTO,INCLUDE_NULL_VALUES, ROOT('Menu')
		FOR JSON PATH,INCLUDE_NULL_VALUES
    );

    SELECT @JsonMenu AS JsonMenu;
END

GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 12/5/2025 11:39:01 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfiles]    Script Date: 12/5/2025 11:40:01 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserProfileById]    Script Date: 12/5/2025 11:41:06 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserProfile]    Script Date: 12/5/2025 11:41:38 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserMenusPagingWithSearch]    Script Date: 12/5/2025 11:42:13 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserMenu]    Script Date: 12/5/2025 11:43:43 PM ******/
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
			INSERT INTO AppUserMenus (Id, Name, IsHeader, IsModule, IsComponent, CssClass, IsRouteLink, RouteLink, RouteLinkClass, Icon, Remark, ParentId, DropdownIcon, SerialNo, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive)
			VALUES (@Id, @Name, @IsHeader, @IsModule, @IsComponent, @CssClass, @IsRouteLink, @RouteLink, @RouteLinkClass, @Icon, @Remark, @ParentId, @DropdownIcon, @SerialNo, @CreatedBy, GETUTCDATE(), NULL, NULL, @IsActive);

			SELECT @@ROWCOUNT AS 'RowsAffected';
		END
        
    END
	ELSE IF @ActionName = 'Update' -- Update
    BEGIN
		IF EXISTS (SELECT 1 FROM AppUserMenus WHERE Id = @Id)
		BEGIN
			UPDATE AppUserMenus SET Name = @Name, IsHeader = @IsHeader,  IsModule = @IsModule, IsComponent = @IsComponent, CssClass = @CssClass, IsRouteLink = @IsRouteLink, RouteLink = @RouteLink,
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteAppUserMenu]    Script Date: 12/5/2025 11:45:07 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserRoleMenuInitialData]    Script Date: 12/5/2025 11:46:01 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetRoleMenusPagingWithSearch]    Script Date: 12/7/2025 4:04:49 AM ******/
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
    DECLARE @Result XML;
    DECLARE @Items XML;

    CREATE TABLE #RoleMenuTBL
    (
        [RowCount] INT,
        [CurrentPage] INT,
        [PageSize] INT,
        [PageCount] INT,
        [Items] XML
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateRoleMenuInBulk]    Script Date: 12/7/2025 4:06:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 07.12.2025
-- Description:	This SP is used to create or update role menu permission for a specific role
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
    -- Return SUCCESS explicitly as scalar value
    -----------------------------------------------------
    SELECT 'SUCCESS' AS Result;
END