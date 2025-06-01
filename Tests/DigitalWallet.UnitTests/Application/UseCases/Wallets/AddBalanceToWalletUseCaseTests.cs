using DigitalWallet.Application.UseCases.Wallets;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.UnitTests.Fixtures;

namespace DigitalWallet.UnitTests.Application.UseCases.Wallets;

public class AddBalanceToWalletUseCaseTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly AddBalanceToWalletUseCase _useCase;

    public AddBalanceToWalletUseCaseTests()
    {
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _useCase = new AddBalanceToWalletUseCase(
            _walletRepositoryMock.Object,
            _transactionRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAddBalanceSuccessfully_WhenValidRequest()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet(currency: Currency.BRL);
        var request = new AddBalanceToWalletUseCase.Request(
            wallet.Id,
            100m,
            "Test deposit",
            "REF-001"
        );

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(wallet.Id)).ReturnsAsync(wallet);

        _transactionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => t);

        _walletRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet w) => w);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.WalletId.Should().Be(wallet.Id);
        result.Value.Amount.Should().Be(100m);
        result.Value.Currency.Should().Be(Currency.BRL.ToString());
        result.Value.NewBalance.Should().Be(100m);
        result.Value.Description.Should().Be("Test deposit");

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(wallet.Id), Times.Once);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenWalletNotFound()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var request = new AddBalanceToWalletUseCase.Request(walletId, 100m, "Test deposit");

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(walletId)).ReturnsAsync((Wallet?)null);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(nameof(Wallet.ValidationError.WalletNotFound));

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(walletId), Times.Once);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await FluentActions
            .Invoking(() => _useCase.ExecuteAsync(null!))
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateCreditTransaction_WithCorrectProperties()
    {
        // Arrange
        var wallet = TestDataFixture.CreateValidWallet(currency: Currency.BRL);
        var addBalanceRequest = new AddBalanceToWalletUseCase.Request(
            wallet.Id,
            50.75m,
            "Salary deposit",
            "SAL-2025-001"
        );

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(wallet.Id)).ReturnsAsync(wallet);

        Transaction? capturedTransaction = null;

        _transactionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Transaction>()))
            .Callback<Transaction>(t => capturedTransaction = t)
            .ReturnsAsync((Transaction t) => t);

        _walletRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet w) => w);

        // Act
        await _useCase.ExecuteAsync(addBalanceRequest);

        // Assert
        capturedTransaction.Should().NotBeNull();
        capturedTransaction!.WalletId.Should().Be(wallet.Id);
        capturedTransaction.Amount.Amount.Should().Be(50.75m);
        capturedTransaction.Amount.Currency.Should().Be(Currency.BRL);
        capturedTransaction.Type.Should().Be(TransactionType.Credit);
        capturedTransaction.Description.Should().Be("Salary deposit");
        capturedTransaction.Reference.Should().Be("SAL-2025-001");
        capturedTransaction.Status.Should().Be(TransactionStatus.Completed);
    }

    [Theory]
    [InlineData(100, 50, 150)]
    [InlineData(0, 100, 100)]
    [InlineData(250.75, 49.25, 300)]
    public async Task ExecuteAsync_ShouldCalculateNewBalanceCorrectly(
        decimal initialBalance,
        decimal addAmount,
        decimal expectedBalance
    )
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallet = TestDataFixture.CreateValidWallet(currency: Currency.BRL);

        // Set initial balance
        if (initialBalance > 0)
        {
            var initialTransaction = TestDataFixture.CreateValidTransaction(
                wallet.Id,
                TestDataFixture.CreateValidMoney(initialBalance, Currency.BRL),
                TransactionType.Credit
            );
            wallet.CalculateBalance(initialTransaction);
        }

        var request = new AddBalanceToWalletUseCase.Request(walletId, addAmount, "Test deposit");

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(walletId)).ReturnsAsync(wallet);

        _transactionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => t);

        _walletRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet w) => w);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.NewBalance.Should().Be(expectedBalance);
    }
}
