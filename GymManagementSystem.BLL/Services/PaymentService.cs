using System.Net.Http;
using System.Text;
using System.Text.Json;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymManagementSystem.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Membership> _membershipRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    public PaymentService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IRepository<Payment> paymentRepository,
        IRepository<Membership> membershipRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<PaymentService>? logger = null)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _paymentRepository = paymentRepository;
        _membershipRepository = membershipRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger ?? NullLogger<PaymentService>.Instance;
    }

    public async Task<PaymobCheckoutResult> CreateCheckoutAsync(int memberId, int membershipId, decimal amount)
    {
        try
        {
            var apiKey = _configuration["Paymob:ApiKey"];
            var hmac = _configuration["Paymob:Hmac"];
            var integrationId = _configuration["Paymob:IntegrationId"];
            var iframeId = _configuration["Paymob:IframeId"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(iframeId) || string.IsNullOrEmpty(integrationId))
                return new PaymobCheckoutResult { Success = false, Error = "Payment not configured." };

            var client = _httpClientFactory.CreateClient();

            var authBody = new { api_key = apiKey };
            var authJson = await client.PostAsync("https://accept.paymob.com/api/auth/tokens",
                new StringContent(JsonSerializer.Serialize(authBody), Encoding.UTF8, "application/json"));

            var authContent = await authJson.Content.ReadAsStringAsync();
            var authDoc = JsonDocument.Parse(authContent);
            var token = authDoc.RootElement.GetProperty("token").GetString();

            if (string.IsNullOrEmpty(token))
                return new PaymobCheckoutResult { Success = false, Error = "Failed to authenticate with Paymob." };

            var orderBody = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (int)(amount * 100),
                currency = "EGP",
                items = new[] { new { } }
            };

            var orderJson = await client.PostAsync("https://accept.paymob.com/api/ecommerce/orders",
                new StringContent(JsonSerializer.Serialize(orderBody), Encoding.UTF8, "application/json"));

            var orderContent = await orderJson.Content.ReadAsStringAsync();
            var orderDoc = JsonDocument.Parse(orderContent);
            var orderId = orderDoc.RootElement.GetProperty("id").GetInt32();

            var paymentBody = new
            {
                auth_token = token,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = "member@example.com",
                    floor = "NA",
                    first_name = "Member",
                    street = "NA",
                    building = "NA",
                    phone_number = "NA",
                    shipping_method = "PKG",
                    postal_code = "NA",
                    city = "NA",
                    country = "EG",
                    last_name = "Member",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(integrationId),
                lock_order_when_paid = "false"
            };

            var paymentKeyJson = await client.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys",
                new StringContent(JsonSerializer.Serialize(paymentBody), Encoding.UTF8, "application/json"));

            var paymentKeyContent = await paymentKeyJson.Content.ReadAsStringAsync();
            var paymentKeyDoc = JsonDocument.Parse(paymentKeyContent);
            var paymentKey = paymentKeyDoc.RootElement.GetProperty("token").GetString();

            var payment = new Payment
            {
                MemberId = memberId,
                MembershipId = membershipId,
                Amount = amount,
                Currency = "EGP",
                Status = "Pending",
                PaymobOrderId = orderId.ToString()
            };

            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            var iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

            return new PaymobCheckoutResult
            {
                Success = true,
                IframeUrl = iframeUrl,
                PaymentId = payment.Id
            };
        }
        catch (Exception ex)
        {
            return new PaymobCheckoutResult { Success = false, Error = ex.Message };
        }
    }

    public async Task<bool> ProcessCallbackAsync(string transactionId, string orderId, string success)
    {
        _logger.LogInformation("Payment callback received: transaction {TransactionId}, order {OrderId}, success {Success}", transactionId, orderId, success);

        var payment = await _paymentRepository.Query()
            .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId);

        if (payment == null) return false;

        if (success == "true")
        {
            payment.Status = "Completed";
            payment.PaymobTransactionId = transactionId;

            if (payment.MembershipId.HasValue)
            {
                var membership = await _membershipRepository.GetByIdAsync(payment.MembershipId.Value);
                if (membership != null)
                {
                    membership.IsActive = true;
                    _membershipRepository.Update(membership);
                }
            }
        }
        else
        {
            payment.Status = "Failed";
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}
