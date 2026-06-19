using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.PL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentController(IPaymentService paymentService, ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _currentUserService = currentUserService;
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
        [FromQuery] string? success)
    {
        if (!string.IsNullOrEmpty(transaction_id) && !string.IsNullOrEmpty(order_id))
        {
            await _paymentService.ProcessCallbackAsync(transaction_id, order_id, success ?? "false");
        }

        if (success == "true")
        {
            TempData["SuccessMessage"] = "Payment successful! Your membership is now active.";
            return RedirectToAction("Index", "Dashboard");
        }

        TempData["ErrorMessage"] = "Payment was not completed.";
        return RedirectToAction("Index", "Plans");
    }
}
