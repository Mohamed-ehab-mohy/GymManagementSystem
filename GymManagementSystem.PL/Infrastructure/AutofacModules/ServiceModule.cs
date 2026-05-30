using Autofac;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.Export;

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
    }
}
