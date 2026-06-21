using System.Text.Json;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

public class ChatbotController : Controller
{
    private const string SessionKey = "ChatHistory";
    private readonly IAiAssistantService _aiService;

    public ChatbotController(IAiAssistantService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromForm] string message)
    {
        var history = GetHistory();

        if (history.Count >= 20)
            return Json(new { response = "Conversation limit reached. Please refresh the page to start a new chat." });

        var reply = await _aiService.GetResponseAsync(message, history);

        history.Add(new ChatMessage("user", message));
        history.Add(new ChatMessage("assistant", reply));
        SaveHistory(history);

        return Json(new { response = reply });
    }

    private List<ChatMessage> GetHistory()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        return json is not null
            ? JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>()
            : new List<ChatMessage>();
    }

    private void SaveHistory(List<ChatMessage> history)
    {
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(history));
    }
}