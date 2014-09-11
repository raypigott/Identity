using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserPhoneNumberStoreTests
    {
        [Test]
        public async void SetPhoneNumberAsync_GivenAUserAndAPhoneNumber_SetsTheUsersPhoneNumber()
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

            await userStore.SetPhoneNumberAsync(user, "0123456789");

            var phoneNumber  = await userStore.GetPhoneNumberAsync(user);

            phoneNumber.Should().Be("0123456789");
        }

        [Test]
        public async void SetPhoneNumberConfirmedAsync_GivenAUserAndATrueFlag_ReturnsTrue()
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

            await userStore.SetPhoneNumberConfirmedAsync(user, true);

            var isPhoneNumberConfirmed = await userStore.GetPhoneNumberConfirmedAsync(user);

            isPhoneNumberConfirmed.Should().BeTrue();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
