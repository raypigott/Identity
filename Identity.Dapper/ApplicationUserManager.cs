using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Identity.Dapper
{
    public class ApplicationUserManager : UserManager<User, int> 
    {
        public ApplicationUserManager(IUserStore<User, int> store)
            : base(store)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var manager = new ApplicationUserManager(new UserStore<User>(applicationDatabaseConfiguration));
            
            manager.UserValidator = new UserValidator<User, int>(manager)
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
                manager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("PasswordReset"));
            }
            return manager;
        }
    }
}
