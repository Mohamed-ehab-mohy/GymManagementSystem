using GymManagementSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

public class ChatbotController : Controller
{
    private readonly IAiAssistantService _aiService;

    public ChatbotController(IAiAssistantService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromForm] string message)
    {
        var response = await _aiService.GetResponseAsync(message);
        return Json(new { response });
    }
}
