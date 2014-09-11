using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
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
                IsLockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "UserName",
                IsAccountActive = true
            };
            await userStore.CreateAsync(user);
            await userStore.SetSecurityStampAsync(user, "stamp");

            var stamp = await userStore.GetSecurityStampAsync(user);

            stamp.Should().Be("stamp");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}