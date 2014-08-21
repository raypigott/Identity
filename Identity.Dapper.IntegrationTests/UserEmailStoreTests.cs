﻿using FluentAssertions;
using Identity.Dapper.Entities;
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

            await userStore.SetEmailAsync(userEntity, "anotheremail@domain.com");

            var emailAddress  = await userStore.GetEmailAsync(userEntity);

            emailAddress.Should().Be("anotheremail@domain.com");
        }

        [Test]
        public async void SetEmailConfirmedAsync_GivenAUserAndATrueFlag_SetsTheEmailAsConfirmed()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<UserEntity>(applicationDatabaseConfiguration);

            var userEntity = new UserEntity
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

            await userStore.CreateAsync(userEntity);

            await userStore.SetEmailConfirmedAsync(userEntity, true);

            var isEmailConfirmed = await userStore.GetEmailConfirmedAsync(userEntity);

            isEmailConfirmed.Should().BeTrue();
        }

        [Test]
        public async void FindByEmailAsync_GivenAnEmailAddress_ReturnsAUser()
        {
            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var userStore = new UserStore<UserEntity>(applicationDatabaseConfiguration);

            var userEntity = new UserEntity
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

            await userStore.CreateAsync(userEntity);

            var user = await userStore.FindByEmailAsync("myname@domain.com");

            user.Should().NotBeNull();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
