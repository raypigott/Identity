namespace Identity.Dapper.TsqlQueries
{
    public class RoleTsql
    {
        public static string GetRole = @"SELECT [Id], [Name] FROM [identity].[Role] WHERE Name = @Name";

        public static string Insert = @"INSERT INTO [identity].[Role]([Name]) VALUES (@Name) SELECT CAST(scope_identity() as int)";

        public static string GetAll = @"SELECT [Id] ,[Name] FROM [identity].[Role]";
    }
}
