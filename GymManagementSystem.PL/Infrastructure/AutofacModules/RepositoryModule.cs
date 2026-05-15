using Autofac;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.PL.Infrastructure.AutofacModules;

public class RepositoryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Repository<>))
               .As(typeof(IRepository<>))
               .InstancePerLifetimeScope();

        builder.RegisterType<PlanRepository>().As<IPlanRepository>().InstancePerLifetimeScope();
        builder.RegisterType<MemberRepository>().As<IMemberRepository>().InstancePerLifetimeScope();
        builder.RegisterType<TrainerRepository>().As<ITrainerRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ClassSessionRepository>().As<IClassSessionRepository>().InstancePerLifetimeScope();
        builder.RegisterType<MembershipRepository>().As<IMembershipRepository>().InstancePerLifetimeScope();
        builder.RegisterType<BookingRepository>().As<IBookingRepository>().InstancePerLifetimeScope();
        builder.RegisterType<HealthRecordRepository>().As<IHealthRecordRepository>().InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
    }
}
