using GymManagementSystem.PL.Infrastructure.AutofacModules;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.PL.Seeders;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ── Bootstrap logger (catches crashes before full config loads) ──────────────
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

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddSingleton<AuditInterceptor>();
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

    builder.Services.AddSingleton<ISingletonService, SingletonService>();
    builder.Services.AddScoped<IScopedService, ScopedService>();
    builder.Services.AddTransient<ITransientService, TransientService>();

    builder.Services.AddHostedService<CleanupBackgroundService>();

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // ── Middleware pipeline ───────────────────────────────────────────────────
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

    // Log every HTTP request/response via Serilog
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.UseStaticFiles();
    app.UseRouting();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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

