using ReStyle.Core.Enums;

namespace ReStyle.Application.DTOs;

public record UserDto(int UserId, string Username, string Email, decimal Balance, UserRole Role, bool IsBlocked, DateTime CreatedAt);

public record RegisterRequest(string Username, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record UpdateProfileRequest(string Username, string Email);

public record ItemDto(int ItemId, string Title, string Description, decimal Price, string? ImagePath, ItemStatus Status, string Category, string Size, DateTime CreatedAt, int UserId, string SellerUsername);

public record CreateItemRequest(string Title, string Description, decimal Price, string? ImagePath, string Category, string Size);

public record UpdateItemRequest(string Title, string Description, decimal Price, string? ImagePath, string Category, string Size);

public record TransactionDto(int TransactionId, int BuyerId, string BuyerUsername, int SellerId, string SellerUsername, int ItemId, string ItemTitle, decimal Amount, DateTime TransactionDate);

public record TopUpRequest(decimal Amount, string CardNumber);

public record BalanceTopUpDto(int TopUpId, decimal Amount, string CardNumberMasked, DateTime CreatedAt);
