namespace ReStyle.Core.Entities;

public class BalanceTopUp
{
    public int TopUpId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string CardNumberMasked { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}
