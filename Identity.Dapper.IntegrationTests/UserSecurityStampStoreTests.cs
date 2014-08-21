using FluentAssertions;
using Identity.Dapper.Entities;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserSecurityStampStoreTests
    {
        [Test]
        public async void SetSecurityStampAsync_GivenAUserAndASecurityStamp_SetsTheStampForTheUser()
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
            await userStore.SetSecurityStampAsync(userEntity, "stamp");

            var stamp = await userStore.GetSecurityStampAsync(userEntity);

            stamp.Should().Be("stamp");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}