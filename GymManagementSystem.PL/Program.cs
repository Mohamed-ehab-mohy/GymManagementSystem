using GymManagementSystem.PL.Infrastructure.AutofacModules;
using GymManagementSystem.PL.Infrastructure.Extensions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.Services;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.PL.Jobs;
using GymManagementSystem.PL.Seeders;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("GymManagementSystem starting up...");

try
{
    var contentRoot = AppContext.BaseDirectory;
    while (!Directory.Exists(Path.Combine(contentRoot, "wwwroot")) && 
           !Directory.Exists(Path.Combine(contentRoot, "GymManagementSystem.PL", "wwwroot")) && 
           Directory.GetParent(contentRoot) != null)
    {
        contentRoot = Directory.GetParent(contentRoot)!.FullName;
    }

    if (!Directory.Exists(Path.Combine(contentRoot, "wwwroot")) && 
        Directory.Exists(Path.Combine(contentRoot, "GymManagementSystem.PL", "wwwroot")))
    {
        contentRoot = Path.Combine(contentRoot, "GymManagementSystem.PL");
    }

    var options = new WebApplicationOptions { Args = args };
    if (Directory.Exists(Path.Combine(contentRoot, "wwwroot")))
    {
        options = new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = contentRoot,
            WebRootPath = Path.Combine(contentRoot, "wwwroot")
        };
    }

    var builder = WebApplication.CreateBuilder(options);

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new RepositoryModule());
        containerBuilder.RegisterModule(new ServiceModule());
    });

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();

        var logsDbConn = context.Configuration.GetConnectionString("LogsDb");
        if (!string.IsNullOrEmpty(logsDbConn))
        {
            configuration.WriteTo.PostgreSQL(
                connectionString: logsDbConn,
                tableName: "Logs",
                columnOptions: (IDictionary<string, Serilog.Sinks.PostgreSQL.ColumnWriters.ColumnWriterBase>?)null,
                needAutoCreateTable: true);
        }
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<AuditInterceptor>();
    builder.Services.AddSingleton<SoftDeleteInterceptor>();

    builder.Services.AddDbContext<GymDbContext>((sp, opts) =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            opts.UseInMemoryDatabase("GymSystemTestDb");
        }
        else
        {
            opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(
                    sp.GetRequiredService<AuditInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>()
                );
        }
    });

    builder.Services.AddHostedService<CleanupBackgroundService>();
    builder.Services.AddHostedService<RenewalReminderService>();

    var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
    if (!string.IsNullOrEmpty(googleClientId))
    {
        authBuilder.AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
            options.CallbackPath = "/signin-google";
            options.Scope.Add("profile");
            options.Scope.Add("email");
        });
    }

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddMapster();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connStr))
{
    builder.Services.AddHealthChecks()
        .AddSqlServer(connStr, name: "SQL Server", tags: new[] { "db", "sqlserver" });
}
else
{
    builder.Services.AddHealthChecks();
}

builder.Services.AddScoped<IPurgeService, PurgeService>();
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("PurgeDeletedRecords");
    q.AddJob<PurgeDeletedRecordsJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("PurgeDeletedRecords-Trigger")
        .WithCronSchedule("0 0 3 * * ?")); // 3 AM daily
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapHub<NotificationHub>("/hubs/notifications");

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    await DatabaseSeeder.SeedAllAsync(app.Services);

    Log.Information("GymManagementSystem started successfully.");
    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "GymManagementSystem terminated unexpectedly.");
    throw;
}
finally
{
    Log.Information("GymManagementSystem shut down.");
    await Log.CloseAndFlushAsync();
}

public partial class Program { }

