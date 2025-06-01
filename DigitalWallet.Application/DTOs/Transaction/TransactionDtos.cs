using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.CrossCutting.Validation;
using static DigitalWallet.Application.UseCases.Wallets.CreateTransferUseCase;

namespace DigitalWallet.Application.DTOs.Transaction;

public class CreateTransactionDto
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class TransferValidationAggregate
{
    public Domain.Entities.Wallet FromWallet { get; set; } = default!;
    public Domain.Entities.Wallet ToWallet { get; set; } = default!;
    public Result<Response> Result { get; set; } = default!;
}
