GO
/****** Object:  UserDefinedFunction [dbo].[GetChildMenus]    Script Date: 11/11/2025 10:45:05 AM ******/
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
            ParentId = @parentId AND AURM.AppUserRoleId = @RoleId AND AURM.IsActive = 1
        FOR JSON PATH
    )
END
GO