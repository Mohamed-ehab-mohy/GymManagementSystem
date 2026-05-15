using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Interfaces;

public interface IClassSessionService
{
    Task<IEnumerable<ClassSession>> GetAllClassSessionsAsync();
    Task<ClassSession?> GetClassSessionByIdAsync(int id);
    Task AddClassSessionAsync(ClassSession classSession);
    Task UpdateClassSessionAsync(ClassSession classSession);
    Task DeleteClassSessionAsync(int id);
}
