using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(100.50, 1)] // BRL = 1
    [InlineData(0, 2)] // USD = 2
    [InlineData(1000000, 3)] // EUR = 3
    public void Money_ShouldCreateValidInstance_WhenValidParametersProvided(
        decimal amount,
        int currencyInt
    )
    {
        // Arrange
        var currency = (Currency)currencyInt;

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Money_ShouldThrowArgumentException_WhenNegativeAmountProvided(decimal amount)
    {
        // Act & Assert
        Action act = () => new Money(amount, Currency.BRL);
        act.Should().Throw<ArgumentException>().WithMessage("Amount cannot be negative*");
    }

    [Fact]
    public void Add_ShouldReturnCorrectSum_WhenSameCurrency()
    {
        // Arrange
        var money1 = new Money(100m, Currency.BRL);
        var money2 = new Money(50m, Currency.BRL);

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be(Currency.BRL);
    }

    [Fact]
    public void Add_ShouldThrowInvalidOperationException_WhenDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100m, Currency.BRL);
        var money2 = new Money(50m, Currency.JPY);

        // Act & Assert
        Action act = () => money1.Add(money2);
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot add otherMoney with different currencies");
    }

    [Fact]
    public void Subtract_ShouldReturnCorrectDifference_WhenSameCurrency()
    {
        // Arrange
        var money1 = new Money(100m, Currency.BRL);
        var money2 = new Money(30m, Currency.BRL);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be(Currency.BRL);
    }

    [Fact]
    public void Subtract_ShouldThrowInvalidOperationException_WhenDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100m, Currency.BRL);
        var money2 = new Money(50m, Currency.USD);

        // Act & Assert
        Action act = () => money1.Subtract(money2);
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract otherMoney with different currencies");
    }

    [Fact]
    public void Subtract_ShouldThrowInvalidOperationException_WhenResultWouldBeNegative()
    {
        // Arrange
        var money1 = new Money(50m, Currency.BRL);
        var money2 = new Money(100m, Currency.BRL);

        // Act & Assert
        Action act = () => money1.Subtract(money2);

        act.Should().Throw<InvalidOperationException>().WithMessage("Insufficient funds");
    }

    [Theory]
    [InlineData(100, 1, 100, 1, true)]
    [InlineData(100, 1, 5, 1, false)]
    [InlineData(100, 1, 100, 2, false)]
    public void Equals_ShouldReturnExpectedResult(
        decimal amount1,
        int currency1Int,
        decimal amount2,
        int currency2Int,
        bool expected
    )
    {
        // Arrange
        var currency1 = (Currency)currency1Int;
        var currency2 = (Currency)currency2Int;
        var money1 = new Money(amount1, currency1);
        var money2 = new Money(amount2, currency2);

        // Act & Assert
        money1.Equals(money2).Should().Be(expected);
        (money1 == money2).Should().Be(expected);
        (money1 != money2).Should().Be(!expected);
    }
}
