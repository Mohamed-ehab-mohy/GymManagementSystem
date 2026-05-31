using System;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.DAL.DbContexts;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

namespace GymManagementSystem.PL.Tests.Infrastructure;

public class FakeAntiforgery : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
    {
        return new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
    }

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
    {
        return new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
    }

    public Task<bool> IsRequestValidAsync(HttpContext httpContext)
    {
        return Task.FromResult(true);
    }

    public void SetCookieTokenAndHeader(HttpContext httpContext)
    {
    }

    public Task ValidateRequestAsync(HttpContext httpContext)
    {
        return Task.CompletedTask;
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GymDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<GymDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            services.AddSingleton<IAntiforgery, FakeAntiforgery>();
        });
    }
}
