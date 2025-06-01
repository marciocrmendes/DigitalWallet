using DigitalWallet.CrossCutting.Enums;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.ValueObjects;
using DigitalWallet.UnitTests.Fixtures;

namespace DigitalWallet.UnitTests.Domain.Entities;

public class TransactionTests
{
    [Fact]
    public void Transaction_ShouldCreateValidInstance_WhenValidParametersProvided()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var amount = new Money(100m, Currency.BRL);
        var type = TransactionType.Credit;
        var description = "Test transaction";
        var reference = "TEST-REF";

        // Act
        var transaction = new Transaction(walletId, amount, type, description, reference);

        // Assert
        transaction.WalletId.Should().Be(walletId);
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(type);
        transaction.Description.Should().Be(description);
        transaction.Reference.Should().Be(reference);
        transaction.Status.Should().Be(TransactionStatus.Pending);
    }

    [Fact]
    public void Transaction_ShouldCreateValidInstance_WhenReferenceIsNull()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var amount = new Money(100m, Currency.BRL);
        var type = TransactionType.Debit;
        var description = "Test transaction";

        // Act
        var transaction = new Transaction(walletId, amount, type, description);

        // Assert
        transaction.WalletId.Should().Be(walletId);
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(type);
        transaction.Description.Should().Be(description);
        transaction.Reference.Should().BeNull();
        transaction.Status.Should().Be(TransactionStatus.Pending);
    }

    [Fact]
    public void MarkAsCompleted_ShouldUpdateStatusAndProcessedAt()
    {
        // Arrange
        var transaction = TestDataFixture.CreateValidTransaction();
        var beforeProcessedAt = DateTime.UtcNow;

        // Act
        transaction.MarkAsCompleted();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.ProcessedAt.Should().BeAfter(beforeProcessedAt);
        transaction.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatusAndProcessedAt()
    {
        // Arrange
        var transaction = TestDataFixture.CreateValidTransaction();
        var beforeProcessedAt = DateTime.UtcNow;

        // Act
        transaction.MarkAsFailed();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Failed);
        transaction.ProcessedAt.Should().BeAfter(beforeProcessedAt);
        transaction.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsCancelled_ShouldUpdateStatusAndProcessedAt()
    {
        // Arrange
        var transaction = TestDataFixture.CreateValidTransaction();
        var beforeProcessedAt = DateTime.UtcNow;

        // Act
        transaction.MarkAsCancelled();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Cancelled);
        transaction.ProcessedAt.Should().BeAfter(beforeProcessedAt);
        transaction.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Transaction_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var transaction = TestDataFixture.CreateValidTransaction();

        // Assert
        transaction.Should().BeAssignableTo<BaseEntity>();
        transaction.Id.Should().NotBe(Guid.Empty);
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(TransactionType.Credit)]
    [InlineData(TransactionType.Debit)]
    public void Transaction_ShouldAcceptValidTransactionTypes(TransactionType type)
    {
        // Arrange & Act
        var transaction = TestDataFixture.CreateValidTransaction(type: type);

        // Assert
        transaction.Type.Should().Be(type);
    }

    [Fact]
    public void Transaction_StatusShouldBeReadOnly_AfterCreation()
    {
        // Arrange
        var transaction = TestDataFixture.CreateValidTransaction();

        // Act & Assert
        transaction.Status.Should().Be(TransactionStatus.Pending);

        // Status can only be changed through specific methods
        typeof(Transaction)
            .GetProperty(nameof(Transaction.Status))!
            .SetMethod!.IsPrivate.Should()
            .BeTrue();
    }
}
