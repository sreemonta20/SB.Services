USE [SBSecurityDB]
GO
/****** Object:  Table [dbo].[SecurityLogs]    Script Date: 26/04/2023 12:36:35 PM ******/
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
/****** Object:  Table [dbo].[UserInfo]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[SaltKey] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[LastLoginAttemptAt] [datetime2](7) NULL,
	[LoginFailedAttemptsCount] [int] NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogin](
	[Id] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[RefreshTokenExpiryTime] [datetime2](7) NULL,
 CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMenu]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenu](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsHeader] [bit] NULL,
	[CssClass] [nvarchar](max) NULL,
	[RouteLink] [nvarchar](max) NULL,
	[RouteLinkClass] [nvarchar](max) NULL,
	[Icon] [nvarchar](max) NULL,
	[Remark] [nvarchar](max) NULL,
	[ParentId] [uniqueidentifier] NULL,
	[DropdownIcon] [nvarchar](max) NULL,
	[SerialNo] [int] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_UserMenu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoleMenu]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoleMenu](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[IsView] [bit] NULL,
	[IsCreate] [bit] NULL,
	[IsUpdate] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_UserRoleMenu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserInfo_UserRole_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[UserRole] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserInfo] CHECK CONSTRAINT [FK_UserInfo_UserRole_RoleId]
GO
ALTER TABLE [dbo].[UserRoleMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleMenu_UserMenu_MenuId] FOREIGN KEY([MenuId])
REFERENCES [dbo].[UserMenu] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoleMenu] CHECK CONSTRAINT [FK_UserRoleMenu_UserMenu_MenuId]
GO
ALTER TABLE [dbo].[UserRoleMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleMenu_UserRole_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[UserRole] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoleMenu] CHECK CONSTRAINT [FK_UserRoleMenu_UserRole_RoleId]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserById]    Script Date: 23/07/2023 6:18:23 PM ******/
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
	SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.RoleId,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfo UI 
	WHERE UI.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUser]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users
-- =============================================
--EXEC SP_GetAllUser 1, 1, true, null
--EXEC SP_GetAllUser 2, 1, true, null
--EXEC SP_GetAllUser 1, 2, true, null
--EXEC SP_GetAllUser 2, 2, true, null
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
	SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.RoleId,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfo UI
	ORDER BY Id ASC OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY  
	RETURN 
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserList]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all users list
-- =============================================
--EXEC SP_GetAllUserList 's','Id','ASC',2,1
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

	SET @QUERY='SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.RoleId,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfo UI '

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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserPagingSearch]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_GetAllUserPagingSearch 'Aa', 1, 10, ''
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
             ,Id
             ,FullName
             ,UserName
             ,NULL AS [Password]
             ,SaltKey
             ,Email
             ,RoleId
             ,LastLoginAttemptAt
             ,LoginFailedAttemptsCount
             ,CreatedBy
             ,CreatedDate
			 ,UpdatedBy
             ,UpdatedDate
      INTO #Results
      FROM UserInfo UI
      WHERE UI.UserName LIKE '%' + ISNULL(@SearchTerm,'') + '%' OR ISNULL(@SearchTerm,'') = ''
    
      SELECT @TotalRecords = COUNT(*)
      FROM #Results
          
      SELECT Id, FullName,UserName,NULL AS [Password],SaltKey,Email,RoleId,LastLoginAttemptAt,
	  LoginFailedAttemptsCount,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
      FROM #Results
      WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1
    
      DROP TABLE #Results
END


GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateUser]    Script Date: 23/07/2023 6:18:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_SaveUpdateUser]
	@ActionName     VARCHAR(10), --Save Update
    @Id				UNIQUEIDENTIFIER,
    @FullName		NVARCHAR(50),
    @UserName		NVARCHAR(50),
	@Password		NVARCHAR(MAX),
	@SaltKey		NVARCHAR(MAX),
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
        INSERT INTO [dbo].[UserInfo] ([Id],[FullName],[UserName],[Password],[SaltKey],[Email],[RoleId],[LastLoginAttemptAt]
           ,[LoginFailedAttemptsCount],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive])
     VALUES
           (@Id,@FullName, @UserName,@Password,@SaltKey, @Email, @RoleId,NULL, 0, @CreatedBy,@CreatedDate,NULL, NULL, @IsActive)

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update' -- Update
    BEGIN
        UPDATE UserInfo SET [FullName] = @FullName, [UserName] = @UserName, [Email] = @Email,[RoleId] = @RoleId
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteUser]    Script Date: 23/07/2023 6:18:23 PM ******/
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
		DELETE FROM [dbo].[UserInfo]
		WHERE Id = @Id
		SELECT @@ROWCOUNT AS 'RowsAffected';
	END
	ELSE
	BEGIN
		UPDATE [dbo].[UserInfo] SET IsActive = 0
		WHERE Id = @Id
		SELECT @@ROWCOUNT AS 'RowsAffected';
	END
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 23/07/2023 6:18:23 PM ******/
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
			 UM.[Id]
            ,UM.[Name]
			,UM.[IsHeader]
			,UM.[CssClass]
			,UM.[RouteLink]
			,UM.[RouteLinkClass]
			,UM.[Icon]
			,UM.[Remark]
			,UM.[ParentId]
			,UM.[DropdownIcon]
			,UM.[SerialNo]
			,UM.[CreatedBy]
			,UM.[CreatedDate]
			,UM.[UpdatedBy]
			,UM.[UpdatedDate]
			,UM.[IsActive]
			,URM.[IsView]
			,URM.[IsCreate]
			,URM.[IsUpdate]
			,URM.[IsDelete]
            ,ISNULL(JSON_QUERY(dbo.GetChildMenus(UM.Id,@RoleID), '$'), '[]') AS Children
        FROM
            UserMenu UM
			INNER JOIN UserRoleMenu URM ON URM.MenuId = UM.Id
			INNER JOIN UserRole UR ON UR.Id = URM.RoleId
        WHERE
            ParentId = @parentId AND URM.RoleId = @RoleId AND URM.IsActive = 1
        FOR JSON PATH
    )
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuByUserId]    Script Date: 23/07/2023 6:18:23 PM ******/
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
 --       dbo.GetChildMenus(UM.Id) AS Children
 --   FROM
 --       UserMenu UM
 --   WHERE
 --       ParentId IS NULL
	--ORDER BY SerialNo
 --   FOR JSON PATH, ROOT('UserMenu');
 --	  SELECT @RoleName = UserRole FROM UserInfo UI WHERE  UI.Id =  @UserId
 --	  SELECT @RoleId = UR.Id FROM UserRole UR WHERE  UR.RoleName = @RoleName
	SELECT @RoleId = RoleId FROM UserInfo UI WHERE  UI.Id =  @UserId
	
	SELECT @JsonMenu = (
        SELECT
         UM.[Id]
		,UM.[Name]
		,UM.[IsHeader]
		,UM.[CssClass]
		,UM.[RouteLink]
		,UM.[RouteLinkClass]
		,UM.[Icon]
		,UM.[Remark]
		,UM.[ParentId]
		,UM.[DropdownIcon]
		,UM.[SerialNo]
		,UM.[CreatedBy]
		,UM.[CreatedDate]
		,UM.[UpdatedBy]
		,UM.[UpdatedDate]
		,UM.[IsActive]
		,URM.[IsView]
		,URM.[IsCreate]
		,URM.[IsUpdate]
		,URM.[IsDelete]
        ,ISNULL(JSON_QUERY(dbo.GetChildMenus(UM.Id,@RoleId), '$'), '[]') AS Children
    FROM
        UserMenu UM
		INNER JOIN UserRoleMenu URM ON URM.MenuId = UM.Id
		INNER JOIN UserRole UR ON UR.Id = URM.RoleId
		WHERE URM.RoleId = @RoleId AND UM.ParentId IS NULL AND URM.IsActive = 1
        
	ORDER BY SerialNo
        --FOR JSON AUTO,INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER
		--FOR JSON AUTO,INCLUDE_NULL_VALUES, ROOT('Menu')
		FOR JSON PATH,INCLUDE_NULL_VALUES
    );

    SELECT @JsonMenu AS JsonMenu;
END


--SELECT URM.Id, URM.RoleId, UR.RoleName, URM.MenuId, UM.Name MenuName, UM.RouteLink,
--	   CASE
--			WHEN UM.IsHeader = 1 AND ISNULL(CAST(UM.ParentId AS VARCHAR(MAX)),'') = '' THEN 'Module'
--			WHEN UM.IsHeader = 0 AND  ISNULL(CAST(UM.ParentId AS VARCHAR(MAX)),'') = '' THEN 'SubModule'
--			ELSE 'Entity'
--	   END AS MenuType,
--	   URM.IsView, URM.IsCreate, URM.IsUpdate, URM.IsDelete,
--URM.CreatedBy, URM.CreatedDate, URM.UpdatedBy, URM.UpdatedDate, URM.IsActive

--FROM UserRoleMenu URM
--INNER JOIN UserRole UR ON UR.Id = URM.RoleId
--INNER JOIN UserMenu UM ON UM.Id = URM.MenuId
--ORDER BY UM.SerialNo
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMenuHierarchyByMenuId]    Script Date: 7/28/2023 11:13:28 AM ******/
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
		 From [UserMenu]
		 Where [Id] = @MenuId
    
		 UNION ALL
    
		 Select	UM.[Id],UM.[Name],UM.[IsHeader] ,UM.[CssClass] ,UM.[RouteLink] ,UM.[RouteLinkClass] ,UM.[Icon] ,UM.[ParentId]
			,UM.[DropdownIcon],UM.[SerialNo] ,UM.[CreatedBy] ,UM.[CreatedDate],UM.[UpdatedBy],UM.[UpdatedDate],UM.[IsActive]
		 From [UserMenu] UM
		 JOIN UserMenuCTE UMC
		 ON UM.[Id] = UMC.[ParentId]
	)

	Select UM1.[Id],UM1.[Name],UM1.[IsHeader] ,UM1.[CssClass] ,UM1.[RouteLink] ,UM1.[RouteLinkClass] ,UM1.[Icon], ISNULL(UM2.[Name], 'No Parent') as ParentName
	From UserMenuCTE UM1
	LEFT Join UserMenuCTE UM2
	ON UM1.[ParentId] = UM2.[Id]
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserMenuPagingWithSearch]    Script Date: 02/08/2023 8:35:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sreemonta Bhowmik
-- Create date: 25.04.2023
-- Description:	Get all user menu list using paging with search
-- =============================================
--EXEC SP_GetAllUserMenuPagingWithSearch '','','',1,1
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

	SET @QUERY='SELECT UM.Id, UM.Name,UM.IsHeader,UM.CssClass,UM.RouteLink,UM.RouteLinkClass,UM.Icon,UM.Remark,
	UM.ParentId,UM.DropdownIcon,UM.SerialNo,UM.CreatedBy,UM.CreatedDate,UM.UpdatedBy,UM.UpdatedDate,UM.IsActive
	FROM UserMenu UM '

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