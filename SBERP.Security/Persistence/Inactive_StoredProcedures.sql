GO
/****** Object:  StoredProcedure [dbo].[SP_CreateUpdateAppUserRoleMenu]    Script Date: 12/5/2025 11:59:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 28.04.2024
-- Description:	This SP is used to assign application user menus to a role or update the application user menus to a certain role.
-- =============================================
ALTER PROCEDURE [dbo].[SP_CreateUpdateAppUserRoleMenu]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfileList]    Script Date: 12/6/2025 12:00:00 AM ******/
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
ALTER     PROCEDURE [dbo].[SP_GetAllAppUserProfileList]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserProfilesPagingSearch]    Script Date: 12/6/2025 12:00:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllAppUserProfilesPagingSearch 's', 1, 10, ''
ALTER PROCEDURE [dbo].[SP_GetAllAppUserProfilesPagingSearch]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllAppUserRoleMenusPagingWithSearch]    Script Date: 12/6/2025 12:00:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user role menu list using paging with search
-- =============================================
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch '','','',1,100
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch 'User','','ASC',1,2
--EXEC SP_GetAllAppUserRoleMenusPagingWithSearch 'Admin','','ASC',1,2 
ALTER PROCEDURE [dbo].[SP_GetAllAppUserRoleMenusPagingWithSearch]
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
/****** Object:  StoredProcedure [dbo].[SP_GetAppUserMenuHierarchyByMenuId]    Script Date: 12/6/2025 12:01:12 AM ******/
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
ALTER PROCEDURE [dbo].[SP_GetAppUserMenuHierarchyByMenuId] 
	@MenuId				UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH UserMenuCTE AS
	(
	     Select	[Id],[Name],[IsHeader] ,[IsModule] ,[IsComponent] ,[CssClass] ,[IsRouteLink] ,[RouteLink] ,[RouteLinkClass] ,[Icon] ,[ParentId]
			,[DropdownIcon],[SerialNo] ,[CreatedBy] ,[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive]
		 From [AppUserMenus]
		 Where [Id] = @MenuId
    
		 UNION ALL
    
		 Select	AUM.[Id],AUM.[Name],AUM.[IsHeader] ,AUM.[IsModule] ,AUM.[IsComponent] ,AUM.[CssClass] ,AUM.[IsRouteLink] ,AUM.[RouteLink] ,AUM.[RouteLinkClass] ,AUM.[Icon] ,AUM.[ParentId]
			,AUM.[DropdownIcon],AUM.[SerialNo] ,AUM.[CreatedBy] ,AUM.[CreatedDate],AUM.[UpdatedBy],AUM.[UpdatedDate],AUM.[IsActive]
		 From [AppUserMenus] AUM
		 JOIN UserMenuCTE AUMC
		 ON AUM.[Id] = AUMC.[ParentId]
	)

	Select AUMGround.[Id],AUMGround.[Name],AUMGround.[IsHeader] ,AUMGround.[IsModule], AUMGround.[IsComponent] , AUMGround.[CssClass] , AUMGround.[IsRouteLink] ,AUMGround.[RouteLink] ,AUMGround.[RouteLinkClass] ,AUMGround.[Icon], 
	ISNULL(AUMLevel.[Name], 'No Parent') as ParentName
	From UserMenuCTE AUMGround
	LEFT Join UserMenuCTE AUMLevel
	ON AUMGround.[ParentId] = AUMLevel.[Id]
END

GO
/****** Object:  StoredProcedure [dbo].[SP_SaveAppUserRoleMenu]    Script Date: 12/6/2025 12:01:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 06.09.2025
-- Description:	This SP is used to save User Role Menu
-- =============================================
ALTER PROCEDURE [dbo].[SP_SaveAppUserRoleMenu]
    @RoleId					UNIQUEIDENTIFIER,
    @RoleMenus				NVARCHAR(MAX), -- JSON string containing MenuId, IsView, IsCreate, IsUpdate, IsDelete, IsActive
	@CreateUpdateBy			NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
	SET XACT_ABORT ON;

    DECLARE @RowsAffected INT = 0;
	BEGIN TRY
		BEGIN TRANSACTION;	
			-- Declare table type to hold role menu data
			DECLARE @TempRoleMenus TABLE (
				MenuId UNIQUEIDENTIFIER,
				IsView BIT,
				IsCreate BIT,
				IsUpdate BIT,
				IsDelete BIT,
				IsActive BIT
			);

			-- Parse JSON into temp table
			INSERT INTO @TempRoleMenus (MenuId, IsView, IsCreate, IsUpdate, IsDelete, IsActive)
			SELECT 
				MenuId,
				IsView,
				IsCreate,
				IsUpdate,
				IsDelete,
				IsActive
			FROM OPENJSON(@RoleMenus)
			WITH (
				MenuId UNIQUEIDENTIFIER '$.MenuId',
				IsView BIT '$.IsView',
				IsCreate BIT '$.IsCreate',
				IsUpdate BIT '$.IsUpdate',
				IsDelete BIT '$.IsDelete',
				IsActive BIT '$.IsDelete'
			);

			-- Delete existing role menu permissions
			DELETE FROM [dbo].[AppUserRoleMenus]
			WHERE AppUserRoleId = @RoleId;

			-- Insert new role menu permissions for components
			INSERT INTO [dbo].[AppUserRoleMenus] (Id, AppUserRoleId, AppUserMenuId, IsView, IsCreate, IsUpdate, IsDelete, CreatedBy, CreatedDate, IsActive)
			SELECT 
				NEWID() AS Id,
				@RoleId AS AppUserRoleId,
				t.MenuId AS AppUserMenuId,
				t.IsView,
				t.IsCreate,
				t.IsUpdate,
				t.IsDelete,
				@CreateUpdateBy CreatedBy,
				GETUTCDATE() CreatedDate,
				t.IsActive
			FROM @TempRoleMenus t;

			-- Identify ancestors (parents and grandparents) to add
			WITH MenuHierarchy AS (
				SELECT 
					m.Id,
					m.ParentId,
					m.IsModule,
					m.IsHeader
				FROM [dbo].[AppUserMenus] m
				WHERE m.Id IN (SELECT MenuId FROM @TempRoleMenus)
				UNION ALL
				SELECT 
					m.Id,
					m.ParentId,
					m.IsModule,
					m.IsHeader
				FROM [dbo].[AppUserMenus] m
				INNER JOIN MenuHierarchy mh ON m.Id = mh.ParentId
				WHERE m.ParentId IS NOT NULL
			)
			INSERT INTO [dbo].[AppUserRoleMenus] (Id, AppUserRoleId, AppUserMenuId, IsView, IsCreate, IsUpdate, IsDelete, CreatedBy, CreatedDate, IsActive)
			SELECT 
				NEWID() AS Id,
				@RoleId AS AppUserRoleId,
				mh.Id AS AppUserMenuId,
				1 AS IsView,
				0 AS IsCreate,
				0 AS IsUpdate,
				0 AS IsDelete,
				@CreateUpdateBy CreatedBy,
				GETUTCDATE() CreatedDate,
				1 AS IsActive
			FROM MenuHierarchy mh
			LEFT JOIN [dbo].[AppUserRoleMenus] rm ON rm.AppUserRoleId = @RoleId AND rm.AppUserMenuId = mh.Id
			WHERE rm.Id IS NULL
			AND (mh.IsModule = 1 OR mh.IsHeader = 1);

		COMMIT TRANSACTION;

        SELECT 1 AS Success, 'Role menu permission saved successfully.' AS Message, @RowsAffected AS RowsAffected;
	END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        SELECT 0 AS Success, @ErrorMessage AS Message, -1 AS RowsAffected;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUserProfile]    Script Date: 12/6/2025 12:01:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_SaveUpdateAppUserProfile]
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