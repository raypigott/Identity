namespace Identity.Dapper.TsqlQueries
{
    public class UserLoginTsql
    {
        public static string Insert = @"INSERT INTO [identity].[UserLogin]
        ([LoginProvider], [ProviderKey], [UserId]) 
            VALUES (@LoginProvider, @ProviderKey, @UserId) SELECT CAST(scope_identity() as int)";

        public static string GetUserId =
            @"SELECT [UserId] FROM [identity].[UserLogin] WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";

        public static string Delete =
            @"Delete from [identity].[UserLogin] where UserId = @UserId and LoginProvider = @LoginProvider and ProviderKey = @ProviderKey";

        public static string GetLogins = @"SELECT [LoginProvider], [ProviderKey] FROM [identity].[UserLogin] WHERE UserId = @UserId";
    }
}