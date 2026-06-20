using GymManagementSystem.Domain;
using GymManagementSystem.PL.ViewModels;
using Mapster;

namespace GymManagementSystem.PL.Infrastructure.Mapping;

public class ViewModelProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Member, MemberViewModel>()
            .Map(d => d.Street, s => s.Address != null ? s.Address.Street : string.Empty)
            .Map(d => d.City, s => s.Address != null ? s.Address.City : string.Empty)
            .Map(d => d.State, s => s.Address != null ? s.Address.State : string.Empty)
            .Map(d => d.ZipCode, s => s.Address != null ? s.Address.ZipCode : string.Empty)
            .Map(d => d.Height, s => s.HealthRecord != null ? s.HealthRecord.Height : 0)
            .Map(d => d.Weight, s => s.HealthRecord != null ? s.HealthRecord.Weight : 0)
            .Map(d => d.BloodType, s => s.HealthRecord != null ? s.HealthRecord.BloodType : string.Empty)
            .Map(d => d.Note, s => s.HealthRecord != null ? s.HealthRecord.Note : (string?)null)
#pragma warning disable CS8603 // Mapster's Ignore returns possible null
            .Ignore(d => d.PhotoFile);
#pragma warning restore CS8603
    }
}
