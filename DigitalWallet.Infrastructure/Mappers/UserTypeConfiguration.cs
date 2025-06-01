using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Infrastructure.Mappers
{
    public sealed class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);

            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);

            builder.Property(e => e.IsActive).HasConversion<int>();

            builder.Property(e => e.CreatedAt).IsRequired();

            builder.Property(e => e.CreatedBy).IsRequired();

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy);

            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Email).IsRequired().HasMaxLength(255);

            builder
                .HasMany(e => e.Wallets)
                .WithOne()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
