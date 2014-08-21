using System.Collections.Generic;
using FluentAssertions;
using Identity.Dapper.Entities;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserRoleTests
    {
        [TestFixtureSetUp]
        public async void SetUp()
        {
            List<RoleEntity> roles = new List<RoleEntity>
            {
                new RoleEntity
                {
                    Name = "Admin"
                },
                new RoleEntity
                {
                    Name = "User"
                },
                new RoleEntity
                {
                    Name = "Manager"
                }
            };

            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var store = new RoleEntityStore(applicationDatabaseConfiguration);

            var entities = await store.Get();

            if (entities.Count == 0)
            {
                foreach (var roleEntity in roles)
                {
                    await store.Insert(roleEntity);
                }
            }
        }

        [Test]
        public async void AddToRoleAsync_GivenAUserAndRole_AddsTheUserToTheRole()
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

            await userStore.AddToRoleAsync(userEntity, "Admin");

            var roles = await userStore.GetRolesAsync(userEntity);

            roles.Should().HaveCount(1);
            roles[0].Should().Be("Admin");
        }

        [Test]
        public async void RemoveFromRoleAsync_GivenAUserAndRole_RemovesTheUserFromTheRole()
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

            await userStore.AddToRoleAsync(userEntity, "User");

            await userStore.RemoveFromRoleAsync(userEntity, "User");

            var roles = await userStore.GetRolesAsync(userEntity);

            roles.Should().HaveCount(0);
        }

        [Test]
        public async void IsInRoleAsync_GivenAUserAndARole_ReturnsTrueIfUserIsInRole()
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

            await userStore.AddToRoleAsync(userEntity, "User");

            var isInRole  = await userStore.IsInRoleAsync(userEntity, "User");

            isInRole.Should().BeTrue();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
