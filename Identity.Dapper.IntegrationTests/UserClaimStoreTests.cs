using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Identity.Dapper.Models;
using Identity.Dapper.Stores;
using NUnit.Framework;

namespace Identity.Dapper.IntegrationTests
{
    [TestFixture]
    public class UserClaimStoreTests
    {
        [Test]
        public async void AddClaimAsync_GivenAUserAndClaim_AddsTheClaim()
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

            await userStore.AddClaimAsync(insertedUser, new Claim("ClaimType", "ClaimValue"));

            IList<Claim> claims = await userStore.GetClaimsAsync(user);

            claims.Should().HaveCount(1);
        }

        [Test]
        public async void RemoveClaimAsync_GivenAUserAndClaim_RemovesTheClaim()
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

            await userStore.AddClaimAsync(insertedUser, new Claim("ClaimType2", "ClaimValue2"));

            await userStore.RemoveClaimAsync(insertedUser, new Claim("ClaimType2", "ClaimValue2"));

            IList<Claim> claims = await userStore.GetClaimsAsync(user);

            claims.Should().HaveCount(0);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}