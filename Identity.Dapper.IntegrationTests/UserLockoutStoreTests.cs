using System;
using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserLockoutStoreTests
    {
        [Test]
        public async void SetLockoutEnabledAsync_GivenAUserAndATrueFlag_LockoutEnabledIsSetToTrue()
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

            await userStore.SetLockoutEnabledAsync(user, true);

            var isLockedOut  = await userStore.GetLockoutEnabledAsync(user);

            isLockedOut.Should().BeTrue();
        }

        [Test]
        public async void SetLockoutEndDateAsync_GivenAUserAndLockoutDate_LockoutDateIsSet()
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

            await userStore.SetLockoutEndDateAsync(user, Convert.ToDateTime("01/01/2014"));

            var lockoutDate = await userStore.GetLockoutEndDateAsync(user);

            lockoutDate.Should().Be(Convert.ToDateTime("01/01/2014"));
        }

        [Test]
        public async void IncrementAccessFailedCountAsync_GivenAUser_IncrementsAccessFailedCount()
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

            await userStore.IncrementAccessFailedCountAsync(user);

            var failedCount = await userStore.GetAccessFailedCountAsync(user);

            failedCount.Should().Be(1);
        }

        [Test]
        public async void ResetAccessFailedCountAsync_GivenAUser_ResetsFailedCountToZero()
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

            await userStore.IncrementAccessFailedCountAsync(user);

            await userStore.ResetAccessFailedCountAsync(user);

            var failedCount = await userStore.GetAccessFailedCountAsync(user);

            failedCount.Should().Be(0);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}