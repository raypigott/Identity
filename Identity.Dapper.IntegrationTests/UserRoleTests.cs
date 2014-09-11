using System.Collections.Generic;
using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserRoleTests
    {
        [TestFixtureSetUp]
        public async void SetUp()
        {
            List<Role> roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin"
                },
                new Role
                {
                    Name = "User"
                },
                new Role
                {
                    Name = "Manager"
                }
            };

            var applicationDatabaseConfiguration = new ApplicationDatabaseConfiguration();
            var store = new RoleStore(applicationDatabaseConfiguration);

            var entities = await store.GetRoles();

            if (entities.Count == 0)
            {
                foreach (var role in roles)
                {
                    await store.Insert(role);
                }
            }
        }

        [Test]
        public async void AddToRoleAsync_GivenAUserAndRole_AddsTheUserToTheRole()
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

            await userStore.AddToRoleAsync(user, "Admin");

            var roles = await userStore.GetRolesAsync(user);

            roles.Should().HaveCount(1);
            roles[0].Should().Be("Admin");
        }

        [Test]
        public async void RemoveFromRoleAsync_GivenAUserAndRole_RemovesTheUserFromTheRole()
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

            await userStore.AddToRoleAsync(user, "User");

            await userStore.RemoveFromRoleAsync(user, "User");

            var roles = await userStore.GetRolesAsync(user);

            roles.Should().HaveCount(0);
        }

        [Test]
        public async void IsInRoleAsync_GivenAUserAndARole_ReturnsTrueIfUserIsInRole()
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

            await userStore.AddToRoleAsync(user, "User");

            var isInRole  = await userStore.IsInRoleAsync(user, "User");

            isInRole.Should().BeTrue();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
