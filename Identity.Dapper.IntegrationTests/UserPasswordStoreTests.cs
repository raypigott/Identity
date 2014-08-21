using FluentAssertions;
using Identity.Dapper.Entities;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserPasswordStoreTests
    {
        [Test]
        public async void SetPasswordHashAsync_GivenAUserAndPasswordHash_SetsTheHashForTheUser()
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
                IsLockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "UserName",
                IsAccountActive = true
            };

            await userStore.CreateAsync(userEntity);

            await userStore.SetPasswordHashAsync(userEntity, "1234");

            var passwordHash = await userStore.GetPasswordHashAsync(userEntity);

            passwordHash.Should().Be("1234");
        }

        [Test]
        public async void HasPasswordAsync_GivenAUserWithAPasswordHash_ReturnsTrue()
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
                IsLockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "UserName",
                IsAccountActive = true
            };

            await userStore.CreateAsync(userEntity);

            var hasPasswordHash = await userStore.HasPasswordAsync(userEntity);

            hasPasswordHash.Should().BeTrue();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
