using Identity.Dapper.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Identity.Dapper
{
    public class ApplicationUserManager : UserManager<UserEntity, int> 
    {
        public ApplicationUserManager(IUserStore<UserEntity, int> store)
            : base(store)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var manager = new ApplicationUserManager(new UserStore<UserEntity>(applicationDatabaseConfiguration));
            
            manager.UserValidator = new UserValidator<UserEntity, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<UserEntity, int>(dataProtectionProvider.Create("PasswordReset"));
            }
            return manager;
        }
    }
}
