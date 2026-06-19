using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface IHealthRecordService
{
    Task<IEnumerable<HealthRecord>> GetAllHealthRecordsAsync();
    Task<HealthRecord?> GetHealthRecordByIdAsync(int id);
    Task AddHealthRecordAsync(HealthRecord healthRecord);
    Task UpdateHealthRecordAsync(HealthRecord healthRecord);
    Task DeleteHealthRecordAsync(int id);
}
