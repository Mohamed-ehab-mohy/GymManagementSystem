using AutoMapper;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.DTOs;

namespace GymManagementSystem.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Member, MemberDto>()
            .ForMember(d => d.Street, o => o.MapFrom(s => s.Address != null ? s.Address.Street : string.Empty))
            .ForMember(d => d.City, o => o.MapFrom(s => s.Address != null ? s.Address.City : string.Empty))
            .ForMember(d => d.Height, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Height : 0))
            .ForMember(d => d.Weight, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Weight : 0))
            .ForMember(d => d.BloodType, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.BloodType : string.Empty))
            .ForMember(d => d.Note, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Note : null));

        CreateMap<Trainer, TrainerDto>();

        CreateMap<Plan, PlanDto>();

        CreateMap<ClassSession, ClassSessionDto>()
            .ForMember(d => d.TrainerName, o => o.MapFrom(s => s.Trainer != null ? $"{s.Trainer.FirstName} {s.Trainer.LastName}" : string.Empty))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.CategoryName : string.Empty))
            .ForMember(d => d.AvailableSlots, o => o.MapFrom(s => s.Capacity - s.Bookings.Count(b => !b.IsDeleted)));

        CreateMap<Category, CategoryDto>();

        CreateMap<Membership, MembershipDto>()
            .ForMember(d => d.MemberName, o => o.MapFrom(s => s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : string.Empty))
            .ForMember(d => d.PlanName, o => o.MapFrom(s => s.Plan != null ? s.Plan.Name : string.Empty));

        CreateMap<Booking, BookingDto>()
            .ForMember(d => d.MemberName, o => o.MapFrom(s => s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : string.Empty))
            .ForMember(d => d.SessionName, o => o.MapFrom(s => s.ClassSession != null ? s.ClassSession.Name : string.Empty));
    }
}
