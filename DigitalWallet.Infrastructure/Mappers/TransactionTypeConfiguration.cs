using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Infrastructure.Mappers
{
    public sealed class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasIndex(e => e.WalletId);
            builder.Property(e => e.WalletId).IsRequired();

            builder.Property(e => e.Type).HasConversion<int>().IsRequired();

            builder.HasIndex(e => e.Status);
            builder.Property(e => e.Status).HasConversion<int>().IsRequired();

            builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);

            builder.Property(e => e.Reference).HasMaxLength(100);

            builder.Property(e => e.ProcessedAt);

            builder.Property(e => e.CreatedAt).IsRequired();

            builder.Property(e => e.CreatedBy).IsRequired();

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy);

            builder.OwnsOne(
                e => e.Amount,
                money =>
                {
                    money
                        .Property(m => m.Amount)
                        .HasColumnName("Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                    money
                        .Property(m => m.Currency)
                        .HasColumnName("Currency")
                        .HasConversion<int>()
                        .IsRequired();
                }
            );
        }
    }
}
