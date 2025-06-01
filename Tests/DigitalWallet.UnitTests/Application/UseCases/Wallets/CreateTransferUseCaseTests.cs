using DigitalWallet.Application.UseCases.Wallets;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Domain.ValueObjects;
using DigitalWallet.UnitTests.Fixtures;
using static DigitalWallet.Application.UseCases.Wallets.CreateTransferUseCase;

namespace DigitalWallet.UnitTests.Application.UseCases.Wallets;

public class CreateTransferUseCaseTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly CreateTransferUseCase _useCase;
    private readonly IFixture _fixture;

    public CreateTransferUseCaseTests()
    {
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _useCase = new CreateTransferUseCase(
            _walletRepositoryMock.Object,
            _transactionRepositoryMock.Object
        );
        _fixture = new Fixture();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidTransfer_ShouldReturnSuccessResult()
    {
        // Arrange
        var fromWallet = TestDataFixture.CreateValidWallet();
        fromWallet.AddCredit(new Money(1000, Currency.BRL));

        var toWallet = TestDataFixture.CreateValidWallet();

        var request = new Request(
            fromWallet.Id,
            toWallet.Id,
            100m,
            Currency.BRL,
            "Test transfer",
            "REF123"
        );

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(fromWallet.Id)).ReturnsAsync(fromWallet);
        _walletRepositoryMock.Setup(x => x.GetByIdAsync(toWallet.Id)).ReturnsAsync(toWallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FromWalletId.Should().Be(fromWallet.Id);
        result.Value.ToWalletId.Should().Be(toWallet.Id);
        result.Value.Amount.Should().Be(100m);
        result.Value.Currency.Should().Be("BRL");
        result.Value.Description.Should().Be("Test transfer");

        IReadOnlyCollection<Wallet> list = [fromWallet, toWallet];

        _walletRepositoryMock.Verify(x => x.UpdateRangeAsync(list), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithSameWalletIds_ShouldReturnFailureResult()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var request = new Request(walletId, walletId, 100m, Currency.BRL, "Test transfer");

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("SameWallet");

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentFromWallet_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Request(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100m,
            Currency.BRL,
            "Test transfer"
        );

        _walletRepositoryMock
            .Setup(x => x.GetByIdAsync(request.FromWalletId))
            .ReturnsAsync((Wallet?)null);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("FromWalletNotFound");

        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentToWallet_ShouldReturnFailureResult()
    {
        // Arrange
        var fromWallet = TestDataFixture.CreateValidWallet();
        var request = new Request(
            fromWallet.Id,
            Guid.NewGuid(),
            100m,
            Currency.BRL,
            "Test transfer"
        );

        _walletRepositoryMock
            .Setup(x => x.GetByIdAsync(request.FromWalletId))
            .ReturnsAsync(fromWallet);
        _walletRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ToWalletId))
            .ReturnsAsync((Wallet?)null);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("ToWalletNotFound");

        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInsufficientBalance_ShouldReturnFailureResult()
    {
        // Arrange
        var fromWallet = TestDataFixture.CreateValidWallet();
        fromWallet.AddCredit(new Money(50, Currency.BRL)); // Less than transfer amount

        var toWallet = TestDataFixture.CreateValidWallet();

        var request = new Request(fromWallet.Id, toWallet.Id, 100m, Currency.BRL, "Test transfer");

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(fromWallet.Id)).ReturnsAsync(fromWallet);
        _walletRepositoryMock.Setup(x => x.GetByIdAsync(toWallet.Id)).ReturnsAsync(toWallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("InsufficientFunds");

        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithDifferentCurrencies_ShouldReturnFailureResult()
    {
        // Arrange
        var fromWallet = TestDataFixture.CreateValidWallet(currency: Currency.BRL);
        fromWallet.AddCredit(new Money(1000, Currency.BRL));

        var toWallet = TestDataFixture.CreateValidWallet(currency: Currency.EUR);

        var request = new Request(
            fromWallet.Id,
            toWallet.Id,
            100m,
            Currency.EUR, // Different from fromWallet currency
            "Test transfer"
        );

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(fromWallet.Id)).ReturnsAsync(fromWallet);
        _walletRepositoryMock.Setup(x => x.GetByIdAsync(toWallet.Id)).ReturnsAsync(toWallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("InvalidCurrency");

        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithZeroAmount_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Request(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m,
            Currency.BRL,
            "Test transfer"
        );

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("InvalidAmount");

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNegativeAmount_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Request(
            Guid.NewGuid(),
            Guid.NewGuid(),
            -100m,
            Currency.BRL,
            "Test transfer"
        );

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("InvalidAmount");

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Wallet>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.ExecuteAsync(null!));
    }
}
