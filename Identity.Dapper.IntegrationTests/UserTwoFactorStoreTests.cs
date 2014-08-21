using FluentAssertions;
using Identity.Dapper.Entities;
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
            var userStore = new UserStore<UserEntity>(applicationDatabaseConfiguration);

            var userEntity = new UserEntity
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
            await userStore.CreateAsync(userEntity);

            await userStore.SetTwoFactorEnabledAsync(userEntity, true);

            var isTwoFactorEnabled = await userStore.GetTwoFactorEnabledAsync(userEntity);

            isTwoFactorEnabled.Should().BeTrue();
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}