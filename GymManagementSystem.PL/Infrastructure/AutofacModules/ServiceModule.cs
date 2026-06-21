using Autofac;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Attendance;
using GymManagementSystem.BLL.Export;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;

namespace GymManagementSystem.PL.Infrastructure.AutofacModules;

public class ServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PlanService>().As<IPlanService>().InstancePerLifetimeScope();
        builder.RegisterType<MemberService>().As<IMemberService>().InstancePerLifetimeScope();
        builder.RegisterType<TrainerService>().As<ITrainerService>().InstancePerLifetimeScope();
        builder.RegisterType<ClassSessionService>().As<IClassSessionService>().InstancePerLifetimeScope();
        builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerLifetimeScope();
        builder.RegisterType<BookingService>().As<IBookingService>().InstancePerLifetimeScope();
        builder.RegisterType<HealthRecordService>().As<IHealthRecordService>().InstancePerLifetimeScope();
        builder.RegisterType<ExportService>().As<IExportService>().InstancePerLifetimeScope();
        builder.RegisterType<AttendanceService>().As<IAttendanceService>().InstancePerLifetimeScope();
        builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerLifetimeScope();
        builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
        builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();
        builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
        builder.RegisterType<AnalyticsService>().As<IAnalyticsService>().InstancePerLifetimeScope();
        builder.RegisterType<CacheService>().As<ICacheService>().SingleInstance();
        builder.Register(c =>
        {
            var config = c.Resolve<Microsoft.Extensions.Configuration.IConfiguration>();
            var apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OpenAI__ApiKey") ?? "";
            return new AiAssistantService(
                c.Resolve<IHttpClientFactory>(),
                apiKey,
                c.Resolve<IPlanRepository>(),
                c.Resolve<IClassSessionRepository>(),
                c.Resolve<ITrainerRepository>()
            );
        }).As<IAiAssistantService>().InstancePerLifetimeScope();
        builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
        builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
        builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope()
            .WithParameter(
                (pi, ctx) => pi.Name == "storagePath",
                (pi, ctx) => Path.Combine(ctx.Resolve<IWebHostEnvironment>().ContentRootPath, "App_Data"));
    }
}
