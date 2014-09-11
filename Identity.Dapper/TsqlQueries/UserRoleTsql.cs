namespace Identity.Dapper.TsqlQueries
{
    public class UserRoleTsql
    {
        public static string Insert = @"INSERT INTO [identity].[UserRole]
           ([UserId]
           ,[RoleId])
            VALUES
           (@UserId
           ,@RoleId)
            SELECT CAST(scope_identity() as int)";

        public static string Delete = @"DELETE FROM [identity].[UserRole]
            WHERE UserId = @UserId AND RoleId = @RoleId";

        public static string GetUserRoles = @"SELECT            
            r.[Name]
            FROM 
            [identity].[UserRole] ur INNER JOIN [identity].[Role] r ON ur.RoleId = r.Id 
            WHERE ur.[UserId] = @UserId";
    }
}
