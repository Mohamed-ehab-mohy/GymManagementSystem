using GymManagementSystem.Domain;
using GymManagementSystem.BLL.DTOs;
using Mapster;

namespace GymManagementSystem.BLL.Mapping;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Member, MemberDto>()
            .Map(d => d.Street, s => s.Address != null ? s.Address.Street : string.Empty)
            .Map(d => d.City, s => s.Address != null ? s.Address.City : string.Empty)
            .Map(d => d.Height, s => s.HealthRecord != null ? s.HealthRecord.Height : 0)
            .Map(d => d.Weight, s => s.HealthRecord != null ? s.HealthRecord.Weight : 0)
            .Map(d => d.BloodType, s => s.HealthRecord != null ? s.HealthRecord.BloodType : string.Empty)
            .Map(d => d.Note, s => s.HealthRecord != null ? s.HealthRecord.Note : null);

        config.NewConfig<Trainer, TrainerDto>();

        config.NewConfig<Plan, PlanDto>();

        config.NewConfig<ClassSession, ClassSessionDto>()
            .Map(d => d.TrainerName, s => s.Trainer != null ? $"{s.Trainer.FirstName} {s.Trainer.LastName}" : string.Empty)
            .Map(d => d.CategoryName, s => s.Category != null ? s.Category.CategoryName : string.Empty)
            .Map(d => d.AvailableSlots, s => s.Capacity - s.Bookings.Count(b => !b.IsDeleted));

        config.NewConfig<Category, CategoryDto>();

        config.NewConfig<Membership, MembershipDto>()
            .Map(d => d.MemberName, s => s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : string.Empty)
            .Map(d => d.PlanName, s => s.Plan != null ? s.Plan.Name : string.Empty);

        config.NewConfig<Booking, BookingDto>()
            .Map(d => d.MemberName, s => s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : string.Empty)
            .Map(d => d.SessionName, s => s.ClassSession != null ? s.ClassSession.Name : string.Empty);
    }
}
