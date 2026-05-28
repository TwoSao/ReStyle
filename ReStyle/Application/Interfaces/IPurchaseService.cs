using ReStyle.Application.DTOs;

namespace ReStyle.Application.Interfaces;

public interface IPurchaseService
{
    Task<(bool Success, string Message)> PurchaseItemAsync(int buyerId, int itemId);
}
