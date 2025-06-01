using DigitalWallet.CrossCutting.Auditory;
using DigitalWallet.CrossCutting.Validation;
using DigitalWallet.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Domain.Entities;

public class User : IdentityUser<Guid>, IIdentifiableEntity, IAuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public virtual ICollection<Role> Roles { get; set; } = [];
    public virtual ICollection<Wallet> Wallets { get; private set; } = [];

    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User()
        : base() { }

    public User(string firstName, string lastName, Email email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        IsActive = true;

        AddWallet(Wallet.CreateDefault(Id, CrossCutting.Enums.Currency.BRL));
    }

    public string FullName => $"{FirstName} {LastName}";

    public void AddWallet(Wallet wallet)
    {
        Wallets.Add(wallet);
    }

    public void ActivateToogle()
    {
        IsActive = !IsActive;
    }

    public class ValidationError
    {
        public static Error UserNotFound() => new(nameof(UserNotFound), "User not found");

        public static Error EmailNotExists() => new(nameof(EmailNotExists), "Email not exists");

        public static Error EmailAlreadyExists() =>
            new(nameof(EmailAlreadyExists), "Email already exists");

        public static Error CreateUserFailed(List<string> list)
        {
            var description = string.Join(", ", list);
            return new Error(nameof(CreateUserFailed), $"Create user failed: {description}");
        }

        public static Error UserInvalidLoginOrPassword()
        {
            return new Error(nameof(UserInvalidLoginOrPassword), "Invalid login or password");
        }
    }
}
