USE [SecurityDB]
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
	[Action] [nvarchar](200) NULL
 CONSTRAINT [PK_AppUserMenusLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
	[Action] [nvarchar](200) NULL
 CONSTRAINT [PK_AppUserProfilesLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
	[Action] [nvarchar](200) NULL
 CONSTRAINT [PK_AppUserRoleMenusLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
	[Action] [nvarchar](200) NULL
 CONSTRAINT [PK_AppUserRolesLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
	[Action] [nvarchar](200) NULL
 CONSTRAINT [PK_AppUsersLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO