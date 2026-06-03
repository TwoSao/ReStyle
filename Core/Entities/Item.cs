using ReStyle.Core.Enums;

namespace ReStyle.Core.Entities;

public class Item
{
    public int ItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.Available;
    public string Category { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public Transaction? Transaction { get; set; }
}
