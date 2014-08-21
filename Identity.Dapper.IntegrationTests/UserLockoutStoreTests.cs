using System;
using FluentAssertions;
using Identity.Dapper.Entities;
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

            await userStore.SetLockoutEnabledAsync(userEntity, true);

            var isLockedOut  = await userStore.GetLockoutEnabledAsync(userEntity);

            isLockedOut.Should().BeTrue();
        }

        [Test]
        public async void SetLockoutEndDateAsync_GivenAUserAndLockoutDate_LockoutDateIsSet()
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

            await userStore.SetLockoutEndDateAsync(userEntity, Convert.ToDateTime("01/01/2014"));

            var lockoutDate = await userStore.GetLockoutEndDateAsync(userEntity);

            lockoutDate.Should().Be(Convert.ToDateTime("01/01/2014"));
        }

        [Test]
        public async void IncrementAccessFailedCountAsync_GivenAUser_IncrementsAccessFailedCount()
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

            await userStore.IncrementAccessFailedCountAsync(userEntity);

            var failedCount = await userStore.GetAccessFailedCountAsync(userEntity);

            failedCount.Should().Be(1);
        }

        [Test]
        public async void ResetAccessFailedCountAsync_GivenAUser_ResetsFailedCountToZero()
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

            await userStore.IncrementAccessFailedCountAsync(userEntity);

            await userStore.ResetAccessFailedCountAsync(userEntity);

            var failedCount = await userStore.GetAccessFailedCountAsync(userEntity);

            failedCount.Should().Be(0);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}