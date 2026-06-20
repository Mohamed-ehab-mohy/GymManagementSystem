using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class ChatbotController : Controller
{
    private readonly IAiAssistantService _aiService;
    private const string SessionKey = "ChatHistory";
    private const int MaxHistory = 20;

    public ChatbotController(IAiAssistantService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromForm] string message)
    {
        var history = GetHistory();
        var response = await _aiService.GetResponseAsync(message, history);
        history.Add(new ChatMessage { Role = "user", Content = message });
        history.Add(new ChatMessage { Role = "assistant", Content = response });
        SaveHistory(history);
        return Json(new { response });
    }

    private List<ChatMessage> GetHistory()
    {
        var data = HttpContext.Session.GetString(SessionKey);
        return string.IsNullOrEmpty(data)
            ? new List<ChatMessage>()
            : System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(data) ?? new();
    }

    private void SaveHistory(List<ChatMessage> history)
    {
        while (history.Count > MaxHistory)
            history.RemoveAt(0);
        HttpContext.Session.SetString(SessionKey, System.Text.Json.JsonSerializer.Serialize(history));
    }
}
