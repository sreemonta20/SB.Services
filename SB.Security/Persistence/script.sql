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

/****** Object:  Table [dbo].[UserInfo]    Script Date: 26/04/2023 12:36:35 PM ******/
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
	[UserRole] [nvarchar](max) NOT NULL,
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
/****** Object:  Table [dbo].[UserLogin]    Script Date: 6/28/2023 4:07:02 PM ******/
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

/****** Object:  Table [dbo].[UserMenu]    Script Date: 13/07/2023 11:09:53 PM ******/
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
/****** Object:  Table [dbo].[UserRole]    Script Date: 13/07/2023 11:10:35 PM ******/
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
/****** Object:  Table [dbo].[UserRoleMenu]    Script Date: 13/07/2023 11:11:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRoleMenu](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUser]    Script Date: 26/04/2023 12:36:35 PM ******/
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
	SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.UserRole,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfo UI
	ORDER BY Id ASC OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY  
	RETURN 
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserList]    Script Date: 26/04/2023 12:36:35 PM ******/
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

	SET @QUERY='SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.UserRole,UI.LastLoginAttemptAt,
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
			OR UserRole LIKE ''%'+@SearchTerm+'%''
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
/****** Object:  StoredProcedure [dbo].[SP_GetAllUserPagingSearch]    Script Date: 26/04/2023 12:36:35 PM ******/
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
             ,UserRole
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
          
      SELECT Id, FullName,UserName,NULL AS [Password],SaltKey,Email,UserRole,LastLoginAttemptAt,
	  LoginFailedAttemptsCount,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate
      FROM #Results
      WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1
    
      DROP TABLE #Results
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserById]    Script Date: 26/04/2023 12:36:35 PM ******/
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
	SELECT UI.Id, UI.FullName,UI.UserName,NULL AS [Password],UI.SaltKey,UI.Email,UI.UserRole,UI.LastLoginAttemptAt,
	UI.LoginFailedAttemptsCount,UI.CreatedBy,UI.CreatedDate,UI.UpdatedBy,UI.UpdatedDate
	FROM UserInfo UI 
	WHERE UI.Id = @Id
END

GO
/****** Object:  StoredProcedure [dbo].[SP_SaveUpdateUser]    Script Date: 26/04/2023 4:57:27 PM ******/
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
	@UserRole		VARCHAR(15),
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
        INSERT INTO [dbo].[UserInfo] ([Id],[FullName],[UserName],[Password],[SaltKey],[Email],[UserRole],[LastLoginAttemptAt]
           ,[LoginFailedAttemptsCount],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[IsActive])
     VALUES
           (@Id,@FullName, @UserName,@Password,@SaltKey, @Email, @UserRole,NULL, 0, @CreatedBy,@CreatedDate,NULL, NULL, @IsActive)

        SELECT @@ROWCOUNT AS 'RowsAffected';
    END
    ELSE IF @ActionName = 'Update' -- Update
    BEGIN
        UPDATE UserInfo SET [FullName] = @FullName, [UserName] = @UserName, [Email] = @Email,[UserRole] = @UserRole
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
/****** Object:  StoredProcedure [dbo].[SP_DeleteUser]    Script Date: 4/26/2023 9:51:02 PM ******/
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