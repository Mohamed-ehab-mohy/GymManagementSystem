using GymManagementSystem.PL.Infrastructure.AutofacModules;
using GymManagementSystem.PL.Infrastructure.Extensions;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.PL.Seeders;
using Microsoft.AspNetCore.Identity;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

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

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<GymDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

    builder.Services.AddAutoMapper(typeof(GymManagementSystem.BLL.Mapping.MappingProfile).Assembly, typeof(Program).Assembly);
    builder.Services.AddControllersWithViews();

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

