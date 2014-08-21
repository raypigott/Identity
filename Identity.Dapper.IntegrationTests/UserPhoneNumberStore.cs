using FluentAssertions;
using Identity.Dapper.Entities;
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

            await userStore.SetPhoneNumberAsync(userEntity, "0123456789");

            var phoneNumber  = await userStore.GetPhoneNumberAsync(userEntity);

            phoneNumber.Should().Be("0123456789");
        }

        [Test]
        public async void SetPhoneNumberConfirmedAsync_GivenAUserAndATrueFlag_ReturnsTrue()
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

            await userStore.SetPhoneNumberConfirmedAsync(userEntity, true);

            var isPhoneNumberConfirmed = await userStore.GetPhoneNumberConfirmedAsync(userEntity);

            isPhoneNumberConfirmed.Should().BeTrue();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.TruncateAllTables();
        }
    }
}
