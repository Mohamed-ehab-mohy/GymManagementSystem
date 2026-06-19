using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public interface IAiAssistantService
{
    Task<string> GetResponseAsync(string message);
}
