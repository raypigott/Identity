namespace Identity.Dapper.TsqlQueries
{
    public class UserClaimTsql
    {
        public static string GetClaimsAsync = @"SELECT [Id]
          ,[UserId]
          ,[ClaimType]
          ,[ClaimValue]
          FROM [identity].[UserClaim]
          WHERE UserId = @UserId";

        public static string AddClaimAsync = @"INSERT INTO [identity].[UserClaim]
           ([UserId]
           ,[ClaimType]
           ,[ClaimValue])
            VALUES
           (@UserId
           ,@ClaimType
           ,@ClaimValue) 
            SELECT CAST(scope_identity() as int)";

        public static string RemoveClaimAsync = @"DELETE FROM [identity].[UserClaim] 
            WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
        
    }
}
