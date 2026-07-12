using System;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace GymManagementSystem.BLL.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _username;
    private readonly string _password;
    private readonly string _baseUrl;

    public EmailService(IConfiguration configuration)
    {
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _fromEmail = configuration["Email:From"] ?? "noreply@gymy.com";
        _username = configuration["Email:Username"] ?? "";
        _password = configuration["Email:Password"] ?? "";
        _baseUrl = configuration["App:BaseUrl"] ?? "http://localhost:5000";
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Power Fitness", _fromEmail));
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

        await SendAsync(message);
    }

    public async Task SendRenewalReminderAsync(string email, int daysLeft, string planName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Power Fitness", _fromEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = daysLeft == 1
            ? "Your Membership Expires TOMORROW!"
            : $"Your Membership Expires in {daysLeft} Days";

        message.Body = new TextPart("html")
        {
            Text = $"""
            <h2>Membership Renewal Reminder</h2>
            <p>Your <strong>{planName}</strong> plan is expiring in <strong>{daysLeft} day(s)</strong>.</p>
            <p>Renew now to keep enjoying unlimited access to Power Fitness!</p>
            <p><a href="{_baseUrl}/Membership/Renew"
                  style="display:inline-block;padding:12px 24px;background:#e63946;color:white;text-decoration:none;border-radius:6px;">
                  Renew Now</a></p>
            <p>If you already renewed, please ignore this email.</p>
            """
        };

        await SendAsync(message);
    }

    private async Task SendAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

        if (!string.IsNullOrEmpty(_username))
            await client.AuthenticateAsync(_username, _password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
