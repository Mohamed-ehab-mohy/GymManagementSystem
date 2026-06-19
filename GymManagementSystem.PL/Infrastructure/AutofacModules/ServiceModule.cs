using Autofac;
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
        builder.RegisterType<CacheService>().As<ICacheService>().SingleInstance();
        builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope()
            .WithParameter(
                (pi, ctx) => pi.Name == "storagePath",
                (pi, ctx) => Path.Combine(ctx.Resolve<IWebHostEnvironment>().ContentRootPath, "App_Data"));
    }
}
