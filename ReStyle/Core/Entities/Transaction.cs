namespace ReStyle.Core.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int ItemId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public User Buyer { get; set; } = null!;
    public User Seller { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
