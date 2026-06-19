using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<Member?> GetMemberByIdAsync(int id);
    Task<PagedResult<Member>> GetPagedMembersAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool ascending = true);
    Task AddMemberAsync(Member member);
    Task UpdateMemberAsync(Member member);
    Task<(bool Success, string Message)> DeleteMemberAsync(int id);
}
