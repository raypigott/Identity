using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserStoreTests
    {
        [Test]
        public async void CreateAsync_GivenNewUser_CreatesNewUserAndAssignsUserId()
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

            var insertedUser = await userStore.FindByIdAsync(user.Id);

            insertedUser.Should().NotBeNull();
            insertedUser.Email.Should().Be("someemail@domain.com");
        }

        [Test]
        public async void UpdateAsync_GivenAnUpdate_UpdatesTheUser()
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

            var existingUser = await userStore.FindByNameAsync("UserName");
            existingUser.Email = "user@domain.com";
            existingUser.PhoneNumber = "1234";

            await userStore.UpdateAsync(existingUser);

            var updatedUser = await userStore.FindByNameAsync("UserName");

            updatedUser.Should().NotBeNull();
            updatedUser.Email.Should().Be("user@domain.com");
            updatedUser.PhoneNumber.Should().Be("1234");
        }

        [Test]
        public async void DeleteAsync_GivenAnExistingUser_UpdatesTheAccountToInActive()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<User>(applicationDatabaseConfiguration);

            var existingUser = await userStore.FindByNameAsync("UserName");
            existingUser.UserName = "HideMe";
            await userStore.UpdateAsync(existingUser);

            await userStore.DeleteAsync(existingUser);

            existingUser.IsAccountActive.Should().BeFalse();
        }

        [Test]
        public async void Users_GivenAUserStore_ReturnsAllActiveUsers()
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

            var users = userStore.Users;
            users.Should().NotBeNullOrEmpty();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
