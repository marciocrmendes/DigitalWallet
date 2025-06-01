using DigitalWallet.Application.UseCases.Wallets;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.UnitTests.Fixtures;

namespace DigitalWallet.UnitTests.Application.UseCases.Wallets;

public class GetWalletBalanceUseCaseTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly GetWalletBalanceUseCase _useCase;

    public GetWalletBalanceUseCaseTests()
    {
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _useCase = new GetWalletBalanceUseCase(_walletRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnSuccess_WhenWalletExists()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallet = TestDataFixture.CreateValidWallet(currency: Currency.BRL);
        var request = new GetWalletBalanceUseCase.Request(walletId);

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(walletId)).ReturnsAsync(wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.WalletId.Should().Be(wallet.Id);
        result.Value.Balance.Should().Be(wallet.Balance.Amount);
        result.Value.Currency.Should().Be(wallet.Balance.Currency.ToString());
        result.Value.Status.Should().Be(wallet.Status.ToString());

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(walletId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenWalletDoesNotExist()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var request = new GetWalletBalanceUseCase.Request(walletId);

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(walletId)).ReturnsAsync((Wallet?)null);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(nameof(Wallet.ValidationError.WalletNotFound));

        _walletRepositoryMock.Verify(x => x.GetByIdAsync(walletId), Times.Once);
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

    [Theory]
    [InlineData(Currency.BRL, 150.75)]
    [InlineData(Currency.BRL, 100.00)]
    [InlineData(Currency.EUR, 200.50)]
    public async Task ExecuteAsync_ShouldReturnCorrectBalance_ForDifferentCurrencies(
        Currency currency,
        decimal amount
    )
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallet = TestDataFixture.CreateValidWallet(currency: currency);

        // Add balance to wallet
        var transaction = TestDataFixture.CreateValidTransaction(
            wallet.Id,
            TestDataFixture.CreateValidMoney(amount, currency),
            TransactionType.Credit
        );
        wallet.CalculateBalance(transaction);

        var request = new GetWalletBalanceUseCase.Request(walletId);

        _walletRepositoryMock.Setup(x => x.GetByIdAsync(walletId)).ReturnsAsync(wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(amount);
        result.Value.Currency.Should().Be(currency.ToString());
    }
}
