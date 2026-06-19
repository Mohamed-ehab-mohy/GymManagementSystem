using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface IClassSessionService
{
    Task<IEnumerable<ClassSession>> GetAllClassSessionsAsync();
    Task<ClassSession?> GetClassSessionByIdAsync(int id);
    Task AddClassSessionAsync(ClassSession classSession);
    Task UpdateClassSessionAsync(ClassSession classSession);
    Task<(bool Success, string Message)> DeleteClassSessionAsync(int id);
}
