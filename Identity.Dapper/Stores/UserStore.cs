using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.ExtensionMethods;
using Identity.Dapper.Models;
using Identity.Dapper.TsqlQueries;
using Microsoft.AspNet.Identity;

namespace Identity.Dapper.Stores
{
    /// <summary>
    /// Responsible for the persistance of a user. Uses an int primary key.
    /// </summary>
    /// <typeparam name="TUser">The user model</typeparam>
    public sealed class UserStore<TUser> : 
        IUserClaimStore<TUser, int>, 
        IUserRoleStore<TUser, int>,
        IUserPasswordStore<TUser, int>,
        IUserSecurityStampStore<TUser, int>, 
        IQueryableUserStore<TUser, int>,
        IUserEmailStore<TUser, int>, 
        IUserPhoneNumberStore<TUser, int>,
        IUserTwoFactorStore<TUser, int>,
        IUserLockoutStore<TUser, int>,
        IUserLoginStore<TUser, int> where TUser : User
    {
        private const string UserArgumentNullExceptionMessage = "User cannot be null";

        private const string UserNameArgumentNullExceptionMessage = "User name cannot be null";

        private const string ClaimArgumentNullExceptionMessage = "Claim cannot be null";

        private const string RoleNameArgumentNullExceptionMessage = "Role name cannot be null or empty";

        private const string PasswordHashArgumentNullExceptionMessage = "Password hash cannot be null or empty";

        private const string StampArgumentNullExceptionMessage = "Security stamp cannot be null or empty";
        
        private const string EmailNullExceptionMessage = "Email cannot be null or empty";

        private const string PhoneNumberNullExceptionMessage = "Phone number cannot be null or empty";

        private const string UserLoginInfoArgumentNullExceptionMessage = "User login info cannot be null or empty";

        private readonly IApplicationDatabaseConfiguration applicationDatabaseConfiguration;

        public UserStore(IApplicationDatabaseConfiguration applicationDatabaseConfiguration)
        {
            this.applicationDatabaseConfiguration = applicationDatabaseConfiguration;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">The user model</param>
        public async Task CreateAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var userId = await sqlConnection.QueryAsync<int>(UserTsql.Insert, user);
                user.Id = userId.Single();
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">The user model</param>
        public Task UpdateAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                sqlConnection.Execute(UserTsql.Update, user);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Renders the user account inactive.
        /// </summary>
        /// <param name="user">The user model</param>
        public async Task DeleteAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);
            
            RemoveAllClaims(user);

            user.IsAccountActive = false;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Remove all the <see cref="Claim"/>s for the user
        /// </summary>
        /// <param name="user"></param>
        private async void RemoveAllClaims(TUser user)
        {
            var claims = await GetClaimsAsync(user);

            foreach (var claim in claims)
            {
                await RemoveClaimAsync(user, claim);
            }
        }

        /// <summary>
        /// Find the user by their userId
        /// </summary>
        /// <param name="userId">The identifier to search on</param>
        /// <returns>The user or null</returns>
        public async Task<TUser> FindByIdAsync(int userId)
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var user = await sqlConnection.QueryAsync<TUser>(UserTsql.FindByIdAsync, new { Id = userId });
                return user.SingleOrDefault();
            }
        }

        /// <summary>
        /// Find the user by username which might not be unique
        /// </summary>
        /// <param name="userName">The identifier to search on</param>
        /// <returns>The first user or null</returns>
        public async Task<TUser> FindByNameAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException(UserNameArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var user = await sqlConnection.QueryAsync<TUser>(UserTsql.FindByNameAsync, new { UserName = userName });
                return user.FirstOrDefault();
            }
        }

        public void Dispose()
        {
            // nothing to dispose.
        }

        /// <summary>
        /// Get the claims for the user
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>A list of claims</returns>
        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var claimsIdentity = new ClaimsIdentity();

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var claims = await sqlConnection.QueryAsync<UserClaim>(UserClaimTsql.GetClaimsAsync, new { userId = user.Id });

                foreach (var userClaim in claims)
                {
                    var claim = new Claim(userClaim.ClaimType, userClaim.ClaimValue);
                    claimsIdentity.AddClaim(claim);
                }

                return claimsIdentity.Claims.ToList();
            }
        }

        /// <summary>
        /// Add a claim
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="claim">The claim</param>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ExecuteClaimQuery(user, claim, UserClaimTsql.AddClaimAsync);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Remove a claim
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="claim">The claim</param>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ExecuteClaimQuery(user, claim, UserClaimTsql.RemoveClaimAsync);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Executes a query using a <see cref="UserClaim"/> as parameters
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="claim">The claim</param>
        /// <param name="tsql">The query</param>
        private async void ExecuteClaimQuery(TUser user, Claim claim, string tsql)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (claim == null) throw new ArgumentNullException(ClaimArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var userClaim = new UserClaim
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };

                sqlConnection.Execute(tsql, userClaim);
            }
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="roleName">The name of the role</param>
        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (roleName.IsNullOrEmpty()) throw new ArgumentNullException(RoleNameArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var roleId = await GetRoleId(roleName);

                if (roleId != null)
                {
                    var userRole = new UserRole
                    {
                        RoleId = roleId.Value,
                        UserId = user.Id
                    };

                    sqlConnection.Execute(UserRoleTsql.Insert, userRole);
                }
            }
        }

        /// <summary>
        /// Get the Role Id
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <returns>The Id or null</returns>
        private async Task<int?> GetRoleId(string roleName)
        {
            if (roleName.IsNullOrEmpty()) throw new ArgumentNullException(RoleNameArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var roleEntities = await sqlConnection.QueryAsync<Role>(RoleTsql.GetRole, new { name = roleName });

                var role = roleEntities.FirstOrDefault();

                if (role != null)
                {
                    return role.Id;
                }

                return null;
            }
        }

        /// <summary>
        /// Remove a user from a role
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="roleName">The role to remove</param>
        public async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (roleName.IsNullOrEmpty()) throw new ArgumentNullException(RoleNameArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var roleId = await GetRoleId(roleName);

                if (roleId != null)
                {
                    var userRole = new UserRole
                    {
                        RoleId = roleId.Value,
                        UserId = user.Id
                    };

                    sqlConnection.Execute(UserRoleTsql.Delete, userRole);
                }
            }
        }

        /// <summary>
        /// Get the roles for a user
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>A list of role names for the user</returns>
        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var roles = await sqlConnection.QueryAsync<string>(UserRoleTsql.GetUserRoles, new { userId = user.Id });

                return roles.ToList();
            }
        }

        /// <summary>
        /// Determine if a user is in a role.
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="roleName">The role to find</param>
        /// <returns>True if the user exists in the role, false otherwise</returns>
        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (roleName.IsNullOrEmpty()) throw new ArgumentNullException(RoleNameArgumentNullExceptionMessage);

            var roles = await GetRolesAsync(user);

            return roles.Contains(roleName);
        }

        /// <summary>
        /// Set the password hash for a user
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="passwordHash">The password hash to set</param>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (passwordHash.IsNullOrEmpty()) throw new ArgumentNullException(PasswordHashArgumentNullExceptionMessage);

            user.PasswordHash = passwordHash;

            UpdateAsync(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the password hash for a user
        /// </summary>
        /// <param name="user">The user model</param>
        ///<returns>The password hash</returns>
        public async Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);

            return foundUser.PasswordHash;
        }

        /// <summary>
        /// Determine if a user has a password
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True if the user has a password, false otherwise</returns>
        public async Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);

            return foundUser.PasswordHash.IsNotNullOrEmpty();
        }

        /// <summary>
        /// Set the security stamp for the user
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="stamp">The stamp</param>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (stamp.IsNullOrEmpty()) throw new ArgumentNullException(StampArgumentNullExceptionMessage);

            user.SecurityStamp = stamp;

            UpdateAsync(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the users security stamp
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The user's security stamp</returns>
        public async Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);

            return foundUser.SecurityStamp;
        }

        /// <summary>
        /// Get the active users
        /// </summary>
        /// <returns>Users</returns>
        private IQueryable<TUser> GetUsers()
        {
            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                sqlConnection.Open();

                var users = sqlConnection.Query<TUser>(UserTsql.Select);
                return  users.AsQueryable();
            }
        }        

        /// <summary>
        /// Get the active users
        /// </summary>
        public IQueryable<TUser> Users
        {
            get
            {
                return GetUsers();
            }
        }

        /// <summary>
        /// Set a users email address
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="email">The email address</param>
        public async Task SetEmailAsync(TUser user, string email)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (email.IsNullOrEmpty()) throw new ArgumentNullException(EmailNullExceptionMessage);

            user.Email = email;

            await UpdateAsync(user);
        }

        /// <summary>
        /// Gets a user's email address
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The email address for the user</returns>
        public async Task<string> GetEmailAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.Email;
        }

        /// <summary>
        /// Determines if a user has confirmed their email address
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>True if the user has confirmed their email address, false otherwise</returns>
        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.IsEmailConfirmed;
        }

        /// <summary>
        /// Sets the user's email confirmation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed">True for confirmed, false otherwise</param>
        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.IsEmailConfirmed = confirmed;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Find a user by email address
        /// </summary>
        /// <param name="email">The email address on which to search.</param>
        /// <returns>The first user with the email address or null</returns>
        public async Task<TUser> FindByEmailAsync(string email)
        {
            if (email.IsNullOrEmpty()) throw new ArgumentNullException(EmailNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var user = await sqlConnection.QueryAsync<TUser>(UserTsql.FindByEmailAsync, new { Email = email });
                return user.FirstOrDefault();
            }
        }

        /// <summary>
        /// Set the user's phone number
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="phoneNumber">The phone number to set</param>
        public async Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (phoneNumber.IsNullOrEmpty()) throw new ArgumentNullException(PhoneNumberNullExceptionMessage);

            user.PhoneNumber = phoneNumber;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Get the user's phone number
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The user's phone number</returns>
        public async Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.PhoneNumber;
        }

        /// <summary>
        /// Determine if the user's phone number has been confirmed
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>True if confirmed, false otherwise</returns>
        public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.IsPhoneNumberConfirmed;
        }

        /// <summary>
        /// Set the user's phone number confirmation
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="confirmed">True for confirmed, false otherwise</param>
        public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.IsPhoneNumberConfirmed = confirmed;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Set the user's two factor authentication
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="enabled">True for enable, false otherwise</param>
        public async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.IsTwoFactorEnabled = enabled;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Determine if the user has two factor authentication enabled
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>True for enabled, false otherwise</returns>
        public async Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.IsTwoFactorEnabled;
        }

        /// <summary>
        /// Get the user's lock out end date
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The lock out end date</returns>
        public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);

            return foundUser.LockoutEndDateUtc.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(foundUser.LockoutEndDateUtc.Value, DateTimeKind.Utc)) : new DateTimeOffset(); 
        }

        /// <summary>
        /// Set a user's lock out end date
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="lockoutEnd">The lock out end date</param>
        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Increment the amount of times the user has failed to authenticate
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The number of failures to authenticate</returns>
        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.AccessFailedCount++;
            await UpdateAsync(user);

            return user.AccessFailedCount;
        }

        /// <summary>
        /// Reset the failed attempt count
        /// </summary>
        /// <param name="user">The user model</param>
        public async Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.AccessFailedCount = 0;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Get the amount of times the user has failed to authenticate
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>The number of failures to authenticate</returns>
        public async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.AccessFailedCount;
        }

        /// <summary>
        /// Determine if a user is locked out of their account
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>true if locked out, false otherwise</returns>
        public async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            var foundUser = await FindByIdAsync(user.Id);
            return foundUser.IsLockoutEnabled;
        }

        /// <summary>
        /// Set the user lockout
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="enabled">True for enabled, false otherwise</param>
        public async Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            user.IsLockoutEnabled = enabled;
            await UpdateAsync(user);
        }

        /// <summary>
        /// Add a user login
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="userLoginInfo">The new login to add</param>
        public async Task AddLoginAsync(TUser user, UserLoginInfo userLoginInfo)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (userLoginInfo == null) throw new ArgumentNullException(UserLoginInfoArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var userLogin = new UserLogin
                {
                    LoginProvider = userLoginInfo.LoginProvider,
                    ProviderKey = userLoginInfo.ProviderKey,
                    UserId = user.Id
                };

                var userLoginId = await sqlConnection.QueryAsync<int>(UserLoginTsql.Insert, userLogin);
            }
        }

        /// <summary>
        /// Remove a user's login
        /// </summary>
        /// <param name="user">The user model</param>
        /// <param name="userLoginInfo">The login to remove</param>
        public async Task RemoveLoginAsync(TUser user, UserLoginInfo userLoginInfo)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            if (userLoginInfo == null) throw new ArgumentNullException(UserLoginInfoArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var userLogin = new UserLogin
                {
                    LoginProvider = userLoginInfo.LoginProvider,
                    ProviderKey = userLoginInfo.ProviderKey,
                    UserId = user.Id
                };

                sqlConnection.Execute(UserLoginTsql.Delete, userLogin);
            }
        }

        /// <summary>
        /// Get all the logins for a user
        /// </summary>
        /// <param name="user">The user model</param>
        /// <returns>A list of a user's logins</returns>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(UserArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var logins = await sqlConnection.QueryAsync<UserLoginInfo>(UserLoginTsql.GetLogins, new { userId = user.Id });

                return logins.ToList();
            }
        }

        /// <summary>
        /// Find a user from a login
        /// </summary>
        /// <param name="userLoginInfo">The login</param>
        /// <returns>The user model</returns>
        public async Task<TUser> FindAsync(UserLoginInfo userLoginInfo)
        {
            if (userLoginInfo == null) throw new ArgumentNullException(UserLoginInfoArgumentNullExceptionMessage);

            using (var sqlConnection = new SqlConnection(applicationDatabaseConfiguration.GetConnectionString()))
            {
                await sqlConnection.OpenAsync();

                var userId = await sqlConnection.QueryAsync<int>(UserLoginTsql.GetUserId, userLoginInfo);

                if (userId == null) return null;

                return await FindByIdAsync(userId.FirstOrDefault());
            }
        }
    }
}