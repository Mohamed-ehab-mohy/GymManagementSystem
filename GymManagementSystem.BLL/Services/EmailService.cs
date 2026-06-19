using System;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace GymManagementSystem.BLL.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var fromEmail = _configuration["Email:From"] ?? "noreply@gymy.com";
        var username = _configuration["Email:Username"] ?? "";
        var password = _configuration["Email:Password"] ?? "";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Power Fitness", fromEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your Password Reset OTP";

        message.Body = new TextPart("html")
        {
            Text = $"""
            <h2>Password Reset</h2>
            <p>Your OTP code is:</p>
            <h1 style="font-size:32px;letter-spacing:6px;color:#e63946;">{otp}</h1>
            <p>This code expires in 10 minutes.</p>
            <p>If you did not request this, please ignore this email.</p>
            """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

        if (!string.IsNullOrEmpty(username))
            await client.AuthenticateAsync(username, password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
