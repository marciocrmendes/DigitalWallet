using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.ValueObjects;
using DigitalWallet.UnitTests.Fixtures;

namespace DigitalWallet.UnitTests.Domain.Entities;

public class WalletTests
{
    [Fact]
    public void Wallet_ShouldCreateValidInstance_WhenValidParametersProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Test Wallet";
        var currency = Currency.BRL;
        var description = "Test Description";

        // Act
        var wallet = new Wallet(userId, name, currency, description);

        // Assert
        wallet.UserId.Should().Be(userId);
        wallet.Name.Should().Be(name);
        wallet.Description.Should().Be(description);
        wallet.Balance.Amount.Should().Be(0);
        wallet.Balance.Currency.Should().Be(currency);
        wallet.Status.Should().Be(WalletStatus.Active);
        wallet.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void CreateDefault_ShouldCreateDefaultWallet()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currency = Currency.BRL;

        // Act
        var wallet = Wallet.CreateDefault(userId, currency);

        // Assert
        wallet.UserId.Should().Be(userId);
        wallet.Name.Should().Be("Default Wallet");
        wallet.Balance.Currency.Should().Be(currency);
        wallet.Status.Should().Be(WalletStatus.Active);
    }

    [Fact]
    public void CalculateBalance_ShouldAddAmount_WhenCreditTransaction()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();
        var creditAmount = new Money(100m, Currency.BRL);
        var transaction = new Transaction(
            wallet.Id,
            creditAmount,
            TransactionType.Credit,
            "Credit test"
        );

        // Act
        wallet.CalculateBalance(transaction);

        // Assert
        wallet.Balance.Amount.Should().Be(100m);
        transaction.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public void CalculateBalance_ShouldSubtractAmount_WhenDebitTransaction()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();
        var initialAmount = new Money(200m, Currency.BRL);
        var creditTransaction = new Transaction(
            wallet.Id,
            initialAmount,
            TransactionType.Credit,
            "Initial credit"
        );
        wallet.CalculateBalance(creditTransaction);

        var debitAmount = new Money(50m, Currency.BRL);
        var debitTransaction = new Transaction(
            wallet.Id,
            debitAmount,
            TransactionType.Debit,
            "Debit test"
        );

        // Act
        wallet.CalculateBalance(debitTransaction);

        // Assert
        wallet.Balance.Amount.Should().Be(150m);
        debitTransaction.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public void CalculateBalance_ShouldThrowArgumentNullException_WhenTransactionIsNull()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();

        // Act & Assert
        Action act = () => wallet.CalculateBalance(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(100, 50, true)] // Sufficient funds
    [InlineData(100, 100, true)] // Exact amount
    [InlineData(100, 150, false)] // Insufficient funds
    public void CanDebit_ShouldReturnExpectedResult(
        decimal balance,
        decimal debitAmount,
        bool expected
    )
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();
        var creditAmount = new Money(balance, Currency.BRL);
        var creditTransaction = new Transaction(
            wallet.Id,
            creditAmount,
            TransactionType.Credit,
            "Setup balance"
        );
        wallet.CalculateBalance(creditTransaction);

        var debitMoney = new Money(debitAmount, Currency.BRL);

        // Act
        var result = wallet.CanDebit(debitMoney);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CanDebit_ShouldReturnFalse_WhenWalletIsInactive()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();
        var creditAmount = new Money(100m, Currency.BRL);
        var creditTransaction = new Transaction(
            wallet.Id,
            creditAmount,
            TransactionType.Credit,
            "Setup balance"
        );
        wallet.CalculateBalance(creditTransaction);
        wallet.UpdateStatus(WalletStatus.Inactive);

        var debitMoney = new Money(50m, Currency.BRL);

        // Act
        var result = wallet.CanDebit(debitMoney);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateWalletStatus()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet();
        var newStatus = WalletStatus.Inactive;

        // Act
        wallet.UpdateStatus(newStatus);

        // Assert
        wallet.Status.Should().Be(newStatus);
        wallet.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Wallet_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var wallet = TestDataFixture.CreateValidWallet();

        // Assert
        wallet.Should().BeAssignableTo<BaseEntity>();
        wallet.Id.Should().NotBe(Guid.Empty);
        wallet.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
