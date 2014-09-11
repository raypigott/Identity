namespace Identity.Dapper.TsqlQueries
{
    public class UserTsql
    {
        public static string Insert = @"INSERT INTO [identity].[User]
           ([Email]
           ,[IsEmailConfirmed]
           ,[PasswordHash]
           ,[SecurityStamp]
           ,[PhoneNumber]
           ,[IsPhoneNumberConfirmed]
           ,[IsTwoFactorEnabled]
           ,[LockoutEndDateUtc]
           ,[IsLockoutEnabled]
           ,[AccessFailedCount]
           ,[UserName]
           ,[IsAccountActive])
     VALUES
           (@Email
           ,@IsEmailConfirmed
           ,@PasswordHash
           ,@SecurityStamp
           ,@PhoneNumber
           ,@IsPhoneNumberConfirmed
           ,@IsTwoFactorEnabled
           ,@LockoutEndDateUtc
           ,@IsLockoutEnabled
           ,@AccessFailedCount
           ,@UserName,
            @IsAccountActive)
            SELECT CAST(scope_identity() as int)";

        public static string Update = @"UPDATE [identity].[User]
           SET [Email] = @Email
              ,[IsEmailConfirmed] = @IsEmailConfirmed
              ,[PasswordHash] = @PasswordHash
              ,[SecurityStamp] = @SecurityStamp
              ,[PhoneNumber] = @PhoneNumber
              ,[IsPhoneNumberConfirmed] = @IsPhoneNumberConfirmed
              ,[IsTwoFactorEnabled] = @IsTwoFactorEnabled
              ,[LockoutEndDateUtc] = @LockoutEndDateUtc
              ,[IsLockoutEnabled] = @IsLockoutEnabled
              ,[AccessFailedCount] = @AccessFailedCount
              ,[UserName] = @UserName
              ,[IsAccountActive] = @IsAccountActive
         WHERE [Id] = @Id";

        public static string FindByIdAsync = @"SELECT [Id]
          ,[Email]
          ,[IsEmailConfirmed]
          ,[PasswordHash]
          ,[SecurityStamp]
          ,[PhoneNumber]
          ,[IsPhoneNumberConfirmed]
          ,[IsTwoFactorEnabled]
          ,[LockoutEndDateUtc]
          ,[IsLockoutEnabled]
          ,[AccessFailedCount]
          ,[UserName]
          ,[IsAccountActive]
        FROM [identity].[User] 
        WHERE Id = @Id AND IsAccountActive = 1";

        public static string FindByNameAsync = @"SELECT [Id]
          ,[Email]
          ,[IsEmailConfirmed]
          ,[PasswordHash]
          ,[SecurityStamp]
          ,[PhoneNumber]
          ,[IsPhoneNumberConfirmed]
          ,[IsTwoFactorEnabled]
          ,[LockoutEndDateUtc]
          ,[IsLockoutEnabled]
          ,[AccessFailedCount]
          ,[UserName]
          ,[IsAccountActive]
        FROM [identity].[User] 
        WHERE UserName = @UserName AND IsAccountActive = 1";

        public static string Select = @"SELECT [Id]
          ,[Email]
          ,[IsEmailConfirmed]
          ,[PasswordHash]
          ,[SecurityStamp]
          ,[PhoneNumber]
          ,[IsPhoneNumberConfirmed]
          ,[IsTwoFactorEnabled]
          ,[LockoutEndDateUtc]
          ,[IsLockoutEnabled]
          ,[AccessFailedCount]
          ,[UserName]
          ,[IsAccountActive]
        FROM [identity].[User] 
        WHERE IsAccountActive = 1";

        public static string FindByEmailAsync = @"SELECT [Id]
          ,[Email]
          ,[IsEmailConfirmed]
          ,[PasswordHash]
          ,[SecurityStamp]
          ,[PhoneNumber]
          ,[IsPhoneNumberConfirmed]
          ,[IsTwoFactorEnabled]
          ,[LockoutEndDateUtc]
          ,[IsLockoutEnabled]
          ,[AccessFailedCount]
          ,[UserName]
          ,[IsAccountActive]
        FROM [identity].[User] 
        WHERE Email = @Email AND IsAccountActive = 1";

    }

    

   

  
}
