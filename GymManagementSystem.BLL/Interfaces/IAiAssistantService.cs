using GymManagementSystem.BLL.Models;

namespace GymManagementSystem.BLL.Interfaces;

public interface IAiAssistantService
{
    Task<string> GetResponseAsync(string message, List<ChatMessage> history);
}
