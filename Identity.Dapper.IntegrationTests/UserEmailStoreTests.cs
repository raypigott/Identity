using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserEmailStoreTests
    {
        [Test]
        public async void SetEmailAsync_GivenAUserAndEmail_SetTheUsersEmail()
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

            await userStore.SetEmailAsync(user, "anotheremail@domain.com");

            var emailAddress  = await userStore.GetEmailAsync(user);

            emailAddress.Should().Be("anotheremail@domain.com");
        }

        [Test]
        public async void SetEmailConfirmedAsync_GivenAUserAndATrueFlag_SetsTheEmailAsConfirmed()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<User>(applicationDatabaseConfiguration);

            var user = new User
            {
                Email = "someemail@domain.com",
                IsEmailConfirmed = false,
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

            await userStore.SetEmailConfirmedAsync(user, true);

            var isEmailConfirmed = await userStore.GetEmailConfirmedAsync(user);

            isEmailConfirmed.Should().BeTrue();
        }

        [Test]
        public async void FindByEmailAsync_GivenAnEmailAddress_ReturnsAUser()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<User>(applicationDatabaseConfiguration);

            var user = new User
            {
                Email = "myname@domain.com",
                IsEmailConfirmed = false,
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

            var foundUser = await userStore.FindByEmailAsync("myname@domain.com");

            foundUser.Should().NotBeNull();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
