using DigitalWallet.Application.UseCases.Wallets;
using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.UnitTests.Fixtures;
using static DigitalWallet.Application.UseCases.Wallets.CreateWalletUseCase;

namespace DigitalWallet.UnitTests.Application.UseCases.Wallets;

public class CreateWalletUseCaseTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateWalletUseCase _useCase;
    private readonly IFixture _fixture;

    public CreateWalletUseCaseTests()
    {
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _useCase = new CreateWalletUseCase(
            _walletRepositoryMock.Object,
            _userRepositoryMock.Object
        );
        _fixture = new Fixture();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(user.Id, "Main Wallet", "My primary wallet", Currency.BRL);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        _walletRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet wallet) => wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Name.Should().Be("Main Wallet");
        result.Value.Description.Should().Be("My primary wallet");
        result.Value.Balance.Should().Be(0);
        result.Value.Currency.Should().Be("BRL");
        result.Value.Status.Should().Be("Active");
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _userRepositoryMock.Verify(x => x.GetByIdAsync(request.UserId), Times.Once);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Request(Guid.NewGuid(), "Main Wallet", "My primary wallet", Currency.BRL);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync((User?)null);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("USER_NOT_FOUND");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(request.UserId), Times.Once);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.ExecuteAsync(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidWalletName_ShouldReturnFailureResult(
        string? walletName
    )
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(user.Id, walletName!, "Description", Currency.BRL);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("INVALID_WALLET_NAME");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithLongWalletName_ShouldReturnFailureResult()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var longName = new string('A', 201); // Assuming max length is 200
        var request = new Request(user.Id, longName, "Description", Currency.BRL);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("INVALID_WALLET_NAME");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithLongDescription_ShouldReturnFailureResult()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var longDescription = new string('A', 501); // Assuming max length is 500
        var request = new Request(user.Id, "Valid Name", longDescription, Currency.BRL);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("INVALID_DESCRIPTION");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullDescription_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(user.Id, "Main Wallet", null, Currency.EUR);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        _walletRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet wallet) => wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Description.Should().BeNull();
        result.Value.Currency.Should().Be("EUR");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(request.UserId), Times.Once);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Once);
    }

    [Theory]
    [InlineData(Currency.BRL)]
    [InlineData(Currency.EUR)]
    [InlineData(Currency.GBP)]
    [InlineData(Currency.JPY)]
    public async Task ExecuteAsync_WithDifferentCurrencies_ShouldReturnSuccessResult(
        Currency currency
    )
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(
            user.Id,
            "Currency Wallet",
            "Wallet for different currency",
            currency
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        _walletRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet wallet) => wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Currency.Should().Be(currency.ToString());

        _userRepositoryMock.Verify(x => x.GetByIdAsync(request.UserId), Times.Once);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesWalletWithCorrectProperties()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(user.Id, "Test Wallet", "Test Description", Currency.BRL);
        Wallet? capturedWallet = null;
        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        _walletRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Wallet>()))
            .Callback<Wallet>(wallet => capturedWallet = wallet)
            .ReturnsAsync((Wallet wallet) => wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        capturedWallet.Should().NotBeNull();
        capturedWallet!.UserId.Should().Be(user.Id);
        capturedWallet.Name.Should().Be("Test Wallet");
        capturedWallet.Description.Should().Be("Test Description");
        capturedWallet.Balance.Currency.Should().Be(Currency.BRL);
        capturedWallet.Balance.Amount.Should().Be(0);
        capturedWallet.Status.Should().Be(WalletStatus.Active);
    }

    [Fact]
    public async Task ExecuteAsync_WithSpecialCharactersInName_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = TestDataFixture.CreateValidUser();
        var request = new Request(
            user.Id,
            "Wallet #1 - Main (USD)",
            "User's primary wallet",
            Currency.BRL
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(request.UserId)).ReturnsAsync(user);

        _walletRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Wallet>()))
            .ReturnsAsync((Wallet wallet) => wallet);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Wallet #1 - Main (USD)");
        result.Value.Description.Should().Be("User's primary wallet");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(request.UserId), Times.Once);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyGuidUserId_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new Request(Guid.Empty, "Wallet Name", "Description", Currency.BRL);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("INVALID_USER_ID");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _walletRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Wallet>()), Times.Never);
    }
}
