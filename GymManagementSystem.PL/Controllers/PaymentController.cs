using System.Security.Cryptography;
using System.Text;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ICurrentUserService currentUserService, IConfiguration configuration, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _currentUserService = currentUserService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(int membershipId, decimal amount)
    {
        var userId = int.TryParse(_currentUserService.UserId, out var id) ? id : 0;
        var result = await _paymentService.CreateCheckoutAsync(userId, membershipId, amount);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error ?? "Payment failed.";
            return RedirectToAction("Index", "Plans");
        }

        return Redirect(result.IframeUrl!);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Callback(
        [FromQuery] string? transaction_id,
        [FromQuery] string? order_id,
        [FromQuery] string? success,
        [FromQuery] string? hmac)
    {
        if (string.IsNullOrEmpty(transaction_id) || string.IsNullOrEmpty(order_id))
        {
            TempData["ErrorMessage"] = "Invalid payment callback.";
            return RedirectToAction("Index", "Plans");
        }

        var paymobHmac = _configuration["Paymob:Hmac"];
        if (!string.IsNullOrEmpty(paymobHmac) && !string.IsNullOrEmpty(hmac))
        {
            var expectedHmac = ComputeHmac(paymobHmac, transaction_id, order_id, success ?? "false");
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedHmac),
                    Encoding.UTF8.GetBytes(hmac)))
            {
                _logger.LogWarning("Payment callback HMAC mismatch for order {OrderId}", order_id);
                TempData["ErrorMessage"] = "Payment verification failed.";
                return RedirectToAction("Index", "Plans");
            }
        }

        await _paymentService.ProcessCallbackAsync(transaction_id, order_id, success ?? "false");

        if (success == "true")
        {
            TempData["SuccessMessage"] = "Payment successful! Your membership is now active.";
            return RedirectToAction("Index", "Dashboard");
        }

        TempData["ErrorMessage"] = "Payment was not completed.";
        return RedirectToAction("Index", "Plans");
    }

    private static string ComputeHmac(string secret, string transactionId, string orderId, string success)
    {
        var payload = $"{transactionId}{orderId}{success}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
