USE [SecurityDB]
GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[GetUserMenuInitialData]    Script Date: 12/24/2023 2:21:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC GetUserMenuInitialData
CREATE PROCEDURE [dbo].[GetUserMenuInitialData]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PMJson NVARCHAR(MAX),
			@CCJson NVARCHAR(MAX),
			@RLJson NVARCHAR(MAX),
			@RLCJson NVARCHAR(MAX),
			@IconJson NVARCHAR(MAX),
			@DDIJson NVARCHAR(MAX),
			@result NVARCHAR(MAX);


    SET @PMJson = (SELECT [Id] id, [Name] name FROM AppUserMenus WHERE [Name] IS NOT NULL FOR JSON AUTO);

	SET @CCJson = (SELECT CssClass id, CssClass name FROM AppUserMenus  WHERE [CssClass] IS NOT NULL FOR JSON AUTO);

	SET @RLJson = (SELECT RouteLink id, RouteLink name FROM AppUserMenus  WHERE [RouteLink] IS NOT NULL FOR JSON AUTO);

	SET @RLCJson = (SELECT RouteLinkClass id, RouteLinkClass name FROM AppUserMenus  WHERE [RouteLinkClass] IS NOT NULL FOR JSON AUTO);

	SET @IconJson = (SELECT Icon id, Icon name FROM AppUserMenus  WHERE [Icon] IS NOT NULL FOR JSON AUTO);

	SET @DDIJson = (SELECT DropdownIcon id, DropdownIcon name FROM AppUserMenus  WHERE [DropdownIcon] IS NOT NULL FOR JSON AUTO);

	SELECT @PMJson AS parentMenu, @CCJson AS cssClass, @RLJson AS routeLink, @RLCJson AS routeLinkClass, @IconJson AS icon, @DDIJson AS dropdownIcon
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END;
GO
/****** Object:  StoredProcedure [dbo].[SP_DeleteUser]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUser]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserList]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuByUserId]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuPagingWithSearch]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserPagingSearch]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetMenuHierarchyByMenuId]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_GetUserById]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUser]    Script Date: 12/24/2023 2:21:57 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateAppUserProfile]    Script Date: 12/24/2023 2:21:57 PM ******/
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
