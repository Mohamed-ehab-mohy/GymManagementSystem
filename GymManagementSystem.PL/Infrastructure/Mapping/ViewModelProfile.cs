using AutoMapper;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;

namespace GymManagementSystem.PL.Infrastructure.Mapping;

public class ViewModelProfile : Profile
{
    public ViewModelProfile()
    {
        CreateMap<Member, MemberViewModel>()
            .ForMember(d => d.Street, o => o.MapFrom(s => s.Address != null ? s.Address.Street : string.Empty))
            .ForMember(d => d.City, o => o.MapFrom(s => s.Address != null ? s.Address.City : string.Empty))
            .ForMember(d => d.State, o => o.MapFrom(s => s.Address != null ? s.Address.State : string.Empty))
            .ForMember(d => d.ZipCode, o => o.MapFrom(s => s.Address != null ? s.Address.ZipCode : string.Empty))
            .ForMember(d => d.Height, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Height : 0))
            .ForMember(d => d.Weight, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Weight : 0))
            .ForMember(d => d.BloodType, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.BloodType : string.Empty))
            .ForMember(d => d.Note, o => o.MapFrom(s => s.HealthRecord != null ? s.HealthRecord.Note : null))
            .ForMember(d => d.PhotoFile, o => o.Ignore());
    }
}
