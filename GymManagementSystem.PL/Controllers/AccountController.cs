using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AccountController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.GoogleAuthEnabled = !string.IsNullOrEmpty(_configuration["Authentication:Google:ClientId"]);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model.Email, model.Password);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        await HttpContext.SignInAsync(result.Value!);

        if (User.IsInRole("Member"))
            return RedirectToAction("Index", "Dashboard");

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.RegisterAsync(
            model.FirstName, model.LastName, model.Email,
            model.Password, model.DateOfBirth, model.Gender);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        var loginResult = await _authService.LoginAsync(model.Email, model.Password);
        if (loginResult.IsSuccess)
            await HttpContext.SignInAsync(loginResult.Value!);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _authService.ForgotPasswordAsync(model.Email);
        TempData["SuccessMessage"] = "If the email exists, an OTP has been sent.";
        return RedirectToAction("ResetPassword", new { email = model.Email });
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        var model = new ResetPasswordViewModel { Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        TempData["SuccessMessage"] = "Password reset successfully. Please sign in.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync("Google");
        if (!result.Succeeded)
            return RedirectToAction("Login");

        var authService = HttpContext.RequestServices.GetRequiredService<IAuthService>();
        var email = result.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        var user = await authService.FindByEmailAsync(email);
        if (user == null)
        {
            var firstName = result.Principal?.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? "Google";
            var lastName = result.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "User";
            var registerResult = await authService.RegisterAsync(email, Guid.NewGuid().ToString("N") + "!Aa1", firstName, lastName);
            if (!registerResult.IsSuccess)
                return RedirectToAction("Login");
            user = await authService.FindByEmailAsync(email);
        }

        if (user != null)
        {
            var principal = await authService.SignInUserAsync(user);
            await HttpContext.SignInAsync(principal);
        }

        return RedirectToAction("Index", "Dashboard");
    }
}
