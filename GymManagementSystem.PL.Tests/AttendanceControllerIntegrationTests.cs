using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Attendance;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.PL.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class AttendanceControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AttendanceControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CheckIn_ValidPayload_RedirectsAndUpdatesDatabase()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        int bookingId;
        string secretKey;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            secretKey = config["AttendanceSettings:SecretKey"]!;

            var booking = await db.Bookings.FirstAsync(b => !b.IsAttended);
            bookingId = booking.Id;
        }

        var validPayload = CheckInPayloadHelper.BuildPayload(bookingId, secretKey);
        var postData = new Dictionary<string, string>
        {
            { "payload", validPayload }
        };
        var content = new FormUrlEncodedContent(postData);

        var response = await client.PostAsync("/Attendance/CheckIn", content);

        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldBe("/Attendance/Scan");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var booking = await db.Bookings.FindAsync(bookingId);
            booking.ShouldNotBeNull();
            booking.IsAttended.ShouldBeTrue();
            booking.CheckedInAt.ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task CheckIn_InvalidSignaturePayload_RedirectsAndDoesNotUpdateDatabase()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        int bookingId;
        string secretKey;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            secretKey = config["AttendanceSettings:SecretKey"]!;

            var booking = await db.Bookings.FirstAsync();
            bookingId = booking.Id;

            booking.IsAttended = false;
            booking.CheckedInAt = null;
            db.Bookings.Update(booking);
            await db.SaveChangesAsync();
        }

        var tamperedPayload = CheckInPayloadHelper.BuildPayload(bookingId, secretKey) + "invalid";
        var postData = new Dictionary<string, string>
        {
            { "payload", tamperedPayload }
        };
        var content = new FormUrlEncodedContent(postData);

        var response = await client.PostAsync("/Attendance/CheckIn", content);

        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldBe("/Attendance/Scan");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var booking = await db.Bookings.FindAsync(bookingId);
            booking.ShouldNotBeNull();
            booking.IsAttended.ShouldBeFalse();
            booking.CheckedInAt.ShouldBeNull();
        }
    }
}
