using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Infrastructure.Mappers
{
    public sealed class WalletTypeConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasIndex(w => w.UserId);
            builder.Property(e => e.UserId).IsRequired();

            builder.Property(e => e.Status).HasConversion<int>();

            builder.Property(e => e.CreatedAt).IsRequired();

            builder.Property(e => e.CreatedBy).IsRequired();

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy);

            builder.OwnsOne(
                e => e.Balance,
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

            builder
                .HasMany(e => e.Transactions)
                .WithOne()
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
