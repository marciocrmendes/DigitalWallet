using DigitalWallet.CrossCutting.Enums;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Domain.ValueObjects;

[Owned]
public class Money
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public Money()
    {
        Amount = 0;
        Currency = Currency.BRL;
    }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = Math.Round(amount, 2);
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add otherMoney with different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                "Cannot subtract otherMoney with different currencies"
            );

        var newAmount = Amount - other.Amount;
        if (newAmount < 0)
            throw new InvalidOperationException("Insufficient funds");

        return new Money(newAmount, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                "Cannot compare otherMoney with different currencies"
            );

        return Amount > other.Amount;
    }

    public bool IsGreaterThanOrEqual(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                "Cannot compare otherMoney with different currencies"
            );

        return Amount >= other.Amount;
    }

    public override bool Equals(object? other)
    {
        return other is Money otherMoney
            && Amount == otherMoney.Amount
            && Currency == otherMoney.Currency;
    }

    public bool Equals(Money other)
    {
        if (other is null)
            return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }

    public static bool operator ==(Money left, Money right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money left, Money right) => !(left == right);
}
