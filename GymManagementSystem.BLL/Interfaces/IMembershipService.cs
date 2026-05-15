using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Interfaces;

public interface IMembershipService
{
    Task<IEnumerable<Membership>> GetAllMembershipsAsync();
    Task<Membership?> GetMembershipByIdAsync(int id);
    Task AddMembershipAsync(Membership membership);
    Task UpdateMembershipAsync(Membership membership);
    Task DeleteMembershipAsync(int id);
}
