using System.Net.Http.Json;
using System.Text.Json;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Models;
using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.BLL.Services;

public class AiAssistantService : IAiAssistantService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly IPlanRepository _planRepo;
    private readonly IClassSessionRepository _sessionRepo;
    private readonly ITrainerRepository _trainerRepo;

    public AiAssistantService(
        IHttpClientFactory httpClientFactory,
        string apiKey,
        IPlanRepository planRepo,
        IClassSessionRepository sessionRepo,
        ITrainerRepository trainerRepo)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = apiKey;
        _model = "gpt-4o-mini";
        _maxTokens = 300;
        _planRepo = planRepo;
        _sessionRepo = sessionRepo;
        _trainerRepo = trainerRepo;
    }

    public async Task<string> GetResponseAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return "AI assistant is not configured. Please set the OpenAI API key.";

        var systemPrompt = await BuildSystemPromptAsync();

        var requestBody = new
        {
            model = _model,
            messages = BuildMessages(systemPrompt, message, history),
            max_tokens = _maxTokens
        };

        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", _apiKey);

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
                   ?? "Sorry, I couldn't process that request.";
        }
        catch (HttpRequestException ex)
        {
            return $"I'm having trouble connecting right now. Please try again later. ({ex.Message})";
        }
    }

    private List<object> BuildMessages(string systemPrompt, string userMessage, List<ChatMessage> history)
    {
        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };

        foreach (var msg in history)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = userMessage });
        return messages;
    }

    private async Task<string> BuildSystemPromptAsync()
    {
        var plans = await _planRepo.GetAllAsync();
        var sessions = await _sessionRepo.GetAllAsync();
        var trainers = await _trainerRepo.GetAllAsync();

        var planList = string.Join("\n", plans.Select(p => $"- {p.Name}: ${p.Price}/{(p.DurationDays >= 30 ? "month" : $"{p.DurationDays} days")}{(p.IsActive ? "" : " (inactive)")}"));
        var today = DateTime.Today;
        var todaySessions = sessions.Where(s => s.StartTime.Date == today).ToList();
        var sessionList = string.Join("\n", todaySessions.Select(s => $"- {s.Name} at {s.StartTime:HH:mm} (capacity: {s.Capacity})"));
        var trainerList = string.Join("\n", trainers.Select(t => $"- {t.FirstName} {t.LastName} ({t.Specialty})"));

        return $@"You are a helpful gym assistant for Power Fitness / GymPro management system.
You must respond in the SAME LANGUAGE as the user's message (Arabic or English).

Current gym context:
- Today's date: {today:yyyy-MM-dd}
- Active plans:\n{planList}
- Today's sessions ({todaySessions.Count}):\n{sessionList}
- Available trainers:\n{trainerList}

Provide concise, accurate answers about gym services, plans, sessions, trainers, bookings, hours (Mon-Fri 6AM-10PM, Sat 7AM-8PM, Sun 8AM-6PM), location (123 Fitness Street, downtown), and facilities.
If you don't know something, suggest the user contact support at support@powerfitness.com.";
    }

    private class OpenAiResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    private class Choice
    {
        public Message? Message { get; set; }
    }

    private class Message
    {
        public string? Content { get; set; }
    }
}
