using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserTwoFactorStoreTests
    {
        [Test]
        public async void SetTwoFactorEnabledAsync_GivenAUserAndATrueFlag_TwoFactorIsEnabled()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<User>(applicationDatabaseConfiguration);

            var user = new User
            {
                Email = "someemail@domain.com",
                IsEmailConfirmed = true,
                PasswordHash = "PasswordHash",
                PhoneNumber = "PhoneNumber",
                IsPhoneNumberConfirmed = true,
                IsTwoFactorEnabled = false,
                LockoutEndDateUtc = null,
                IsLockoutEnabled = false,
                AccessFailedCount = 0,
                UserName = "UserName",
                IsAccountActive = true
            };
            await userStore.CreateAsync(user);

            await userStore.SetTwoFactorEnabledAsync(user, true);

            var isTwoFactorEnabled = await userStore.GetTwoFactorEnabledAsync(user);

            isTwoFactorEnabled.Should().BeTrue();
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}