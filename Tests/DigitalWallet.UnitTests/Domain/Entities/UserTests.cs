using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.ValueObjects;
using DigitalWallet.UnitTests.Fixtures;

namespace DigitalWallet.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldCreateValidInstance_WhenValidParametersProvided()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@test.com");

        // Act
        var user = new User(firstName, lastName, email);

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email.Value);
        user.UserName.Should().Be(email.Value);
        user.IsActive.Should().BeTrue();
        user.Wallets.Should().HaveCount(1);
        user.Wallets.First().Name.Should().Be("Default Wallet");
    }

    [Fact]
    public void FullName_ShouldReturnConcatenatedName()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser(firstName: "John", lastName: "Doe");

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void AddWallet_ShouldAddWalletToCollection()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var initialWalletCount = user.Wallets.Count;
        var newWallet = TestDataFixture.CreateValidWallet(user.Id, Currency.BRL, "USD Wallet");

        // Act
        user.AddWallet(newWallet);

        // Assert
        user.Wallets.Should().HaveCount(initialWalletCount + 1);
        user.Wallets.Should().Contain(newWallet);
    }

    [Fact]
    public void ActivateToogle_ShouldToggleUserActiveStatus()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var initialStatus = user.IsActive;

        // Act
        user.ActivateToogle();

        // Assert
        user.IsActive.Should().Be(!initialStatus);
    }

    [Fact]
    public void ActivateToogle_ShouldToggleBackToOriginalStatus()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var initialStatus = user.IsActive;

        // Act
        user.ActivateToogle();
        user.ActivateToogle();

        // Assert
        user.IsActive.Should().Be(initialStatus);
    }

    [Fact]
    public void User_ShouldInheritFromIdentityUser()
    {
        // Arrange & Act
        var user = TestDataFixture.CreateValidUser();

        // Assert
        user.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser<Guid>>();
    }

    [Fact]
    public void User_ShouldImplementAuditableEntity()
    {
        // Arrange & Act
        var user = TestDataFixture.CreateValidUser();

        // Assert
        user.Should().BeAssignableTo<DigitalWallet.CrossCutting.Auditory.IAuditableEntity>();
        user.Should().BeAssignableTo<DigitalWallet.CrossCutting.Auditory.IIdentifiableEntity>();
    }
}
