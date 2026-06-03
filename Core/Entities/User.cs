using ReStyle.Core.Enums;

namespace ReStyle.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsBlocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<Transaction> BuyerTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> SellerTransactions { get; set; } = new List<Transaction>();
    public ICollection<BalanceTopUp> BalanceTopUps { get; set; } = new List<BalanceTopUp>();
}
