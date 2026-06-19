using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public class PaymobCheckoutResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? IframeUrl { get; set; }
    public int? PaymentId { get; set; }
}

public interface IPaymentService
{
    Task<PaymobCheckoutResult> CreateCheckoutAsync(int memberId, int membershipId, decimal amount);
    Task<bool> ProcessCallbackAsync(string transactionId, string orderId, string success);
}
