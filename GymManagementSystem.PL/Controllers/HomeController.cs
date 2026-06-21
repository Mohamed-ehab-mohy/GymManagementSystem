using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using GymManagementSystem.PL.Models;

namespace GymManagementSystem.PL.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        if (statusCode.HasValue)
        {
            model.StatusCode = statusCode.Value;
            (model.Title, model.Message) = statusCode.Value switch
            {
                404 => ("Page Not Found",
                        "The page you're looking for doesn't exist or has been moved."),
                403 => ("Access Denied",
                        "You don't have permission to access this resource."),
                401 => ("Unauthorized",
                        "Please log in to access this resource."),
                _ => ("HTTP Error",
                      $"An HTTP {statusCode.Value} error occurred.")
            };

            _logger.LogWarning("HTTP {StatusCode} error for request {RequestId}", statusCode.Value, model.RequestId);
        }
        else
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionFeature?.Error is not null)
            {
                var ex = exceptionFeature.Error;
                model.StatusCode = 500;
                model.Title = "Server Error";
                model.Message = "An unexpected error occurred. Our team has been notified.";

                _logger.LogError(ex, "Unhandled exception for request {RequestId}", model.RequestId);

                if (HttpContext.RequestServices
                        .GetRequiredService<IWebHostEnvironment>()
                        .IsDevelopment())
                {
                    model.ExceptionDetails = $"{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}";
                }
            }
        }

        return View(model);
    }
}
