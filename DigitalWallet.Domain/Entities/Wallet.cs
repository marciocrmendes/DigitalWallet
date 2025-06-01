using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;

public class Wallet : BaseEntity
{
    private Wallet()
        : base() { }

    public Wallet(Guid userId, string name, Currency currency, string? description = null)
        : base()
    {
        UserId = userId;
        Name = name;
        Description = description;
        Balance = new Money(0, currency);
        Status = WalletStatus.Active;
    }

    public Guid UserId { get; private set; }
    public string Name { get; private set; } = "Default Wallet";
    public string? Description { get; private set; }
    public Money Balance { get; private set; } = new Money(0, Currency.BRL);
    public WalletStatus Status { get; private set; }

    public virtual ICollection<Transaction> Transactions { get; private set; } = [];

    public static Wallet CreateDefault(Guid userId, Currency currency) =>
        new(userId, "Default Wallet", currency);

    public void CalculateBalance(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction.Type == TransactionType.Credit)
        {
            Balance = Balance.Add(transaction.Amount);
        }
        else if (transaction.Type == TransactionType.Debit)
        {
            Balance = Balance.Subtract(transaction.Amount);
        }

        transaction.MarkAsCompleted();

        UpdateTimestamp();
    }

    public bool CanDebit(Money amount) =>
        Status == WalletStatus.Active && Balance.Amount >= amount.Amount;

    public void UpdateStatus(WalletStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        Status = WalletStatus.Inactive;
        UpdateTimestamp();
    }

    public void AddCredit(Money money)
    {
        Balance = Balance.Add(money);

        var creditTransaction = new Transaction(
            Id,
            money,
            TransactionType.Credit,
            "Credit transaction"
        );
        creditTransaction.MarkAsCompleted();

        Transactions.Add(creditTransaction);

        UpdateTimestamp();
    }

    public class ValidationError
    {
        public static Error WalletNotFound() => new(nameof(WalletNotFound), "Wallet not found");

        public static Error InsufficientFunds() =>
            new(nameof(InsufficientFunds), "Insufficient funds in the wallet");

        public static Error InvalidCurrency() =>
            new(nameof(InvalidCurrency), "Invalid currency for the wallet");

        public static Error InvalidWalletName() =>
            new(
                "INVALID_WALLET_NAME",
                "Wallet name cannot be null, empty, or longer than 200 characters"
            );

        public static Error InvalidDescription() =>
            new("INVALID_DESCRIPTION", "Description cannot be longer than 500 characters");

        public static Error InvalidUserId() => new("INVALID_USER_ID", "User ID cannot be empty");

        public static Error UserNotFound() => new("USER_NOT_FOUND", "User not found");
    }
}
