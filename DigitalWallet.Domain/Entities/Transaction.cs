using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.ValueObjects;

namespace DigitalWallet.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid WalletId { get; set; }
    public Money Amount { get; set; } = new Money(0, Currency.BRL);
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private Transaction()
        : base() { }

    public Transaction(
        Guid walletId,
        Money amount,
        TransactionType type,
        string description,
        string? reference = null
    )
        : base()
    {
        WalletId = walletId;
        Amount = amount;
        Type = type;
        Description = description;
        Reference = reference;
        Status = TransactionStatus.Pending;
    }

    public void MarkAsCompleted()
    {
        Status = TransactionStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarkAsFailed()
    {
        Status = TransactionStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarkAsCancelled()
    {
        Status = TransactionStatus.Cancelled;
        ProcessedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public class ValidationError
    {
        public static Error InvalidAmount() =>
            new(nameof(InvalidAmount), "Amount must be greater than zero");

        public static Error SameWallet() =>
            new(nameof(SameWallet), "Cannot transfer to the same wallet");

        public static Error FromWalletNotFound() =>
            new(nameof(FromWalletNotFound), "Source wallet not found");

        public static Error ToWalletNotFound() =>
            new(nameof(ToWalletNotFound), "Destination wallet not found");
    }
}
