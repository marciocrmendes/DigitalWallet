using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.UnitTests.Fixtures;

public static class TestDataFixture
{
    public static User CreateValidUser(
        string? email = null,
        string? firstName = null,
        string? lastName = null
    )
    {
        var user = new User(
            firstName ?? "John",
            lastName ?? "Doe",
            new Email(email ?? "john.doe@test.com")
        );

        // Set a valid GUID for testing purposes
        user.Id = Guid.NewGuid();

        return user;
    }

    public static Wallet CreateValidWallet(
        Guid? userId = null,
        Currency currency = Currency.BRL,
        string? name = null
    )
    {
        return new Wallet(
            userId ?? Guid.NewGuid(),
            name ?? "Test Wallet",
            currency,
            "Test wallet description"
        );
    }

    public static Money CreateValidMoney(decimal amount = 100m, Currency currency = Currency.BRL)
    {
        return new Money(amount, currency);
    }

    public static Transaction CreateValidTransaction(
        Guid? walletId = null,
        Money? amount = null,
        TransactionType type = TransactionType.Credit,
        string? description = null,
        string? reference = null
    )
    {
        return new Transaction(
            walletId ?? Guid.NewGuid(),
            amount ?? CreateValidMoney(),
            type,
            description ?? "Test transaction",
            reference ?? "TEST-REF"
        );
    }

    public static Email CreateValidEmail(string? email = null)
    {
        return new Email(email ?? "test@example.com");
    }
}
