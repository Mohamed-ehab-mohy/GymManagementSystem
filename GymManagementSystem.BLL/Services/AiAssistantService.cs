using System.ClientModel;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = GymManagementSystem.BLL.Models.ChatMessage;

namespace GymManagementSystem.BLL.Services;

public class AiAssistantService : IAiAssistantService
{
    private readonly IPlanRepository _planRepo;
    private readonly IClassSessionRepository _sessionRepo;
    private readonly ITrainerRepository _trainerRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<AiAssistantService> _logger;
    private readonly ChatClient _chatClient;

    public AiAssistantService(
        IPlanRepository planRepo,
        IClassSessionRepository sessionRepo,
        ITrainerRepository trainerRepo,
        IConfiguration config,
        ILogger<AiAssistantService> logger)
    {
        _planRepo = planRepo;
        _sessionRepo = sessionRepo;
        _trainerRepo = trainerRepo;
        _config = config;
        _logger = logger;

        var apiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured. Set it via User Secrets or appsettings.");
        var model = config["OpenAI:Model"] ?? "gpt-4o-mini";

        _chatClient = new ChatClient(model, new ApiKeyCredential(apiKey));
    }

    public async Task<string> GetResponseAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Please ask me something!";

        var systemPrompt = await BuildSystemPromptAsync();

        var messages = new List<ChatMessage> { new("system", systemPrompt) };
        messages.AddRange(history.TakeLast(20));
        messages.Add(new ChatMessage("user", message));

        var openAiMessages = messages.Select<ChatMessage, OpenAI.Chat.ChatMessage>(m => m.Role switch
        {
            "system" => new SystemChatMessage(m.Content),
            "user" => new UserChatMessage(m.Content),
            "assistant" => new AssistantChatMessage(m.Content),
            _ => new UserChatMessage(m.Content)
        }).ToList();

        try
        {
            var maxTokens = _config.GetValue<int>("OpenAI:MaxTokens", 500);
            var temperature = _config.GetValue<float>("OpenAI:Temperature", 0.7f);

            var completion = await _chatClient.CompleteChatAsync(openAiMessages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = maxTokens
            });

            var reply = completion.Value.Content[0].Text;
            _logger.LogInformation("Chat: user={UserMessage} -> assistant={AssistantReply}", message, reply);
            return reply;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI API call failed for message: {Message}", message);
            return "Sorry, I'm having trouble connecting to my brain right now. Please try again in a moment.";
        }
    }

    private async Task<string> BuildSystemPromptAsync()
    {
        var plans = (await _planRepo.GetAllAsync()).Where(p => p.IsActive).ToList();
        var sessions = (await _sessionRepo.GetAllAsync()).Where(s => s.ScheduleTime.Date == DateTime.Today && !s.IsDeleted).ToList();
        var trainers = (await _trainerRepo.GetAllAsync()).Where(t => !t.IsDeleted).ToList();

        var plansSummary = plans.Count != 0
            ? string.Join("\n", plans.Select(p => $"- {p.Name}: {p.Price:C} for {p.DurationDays} days{(p.Description != null ? $" — {p.Description}" : "")}"))
            : "- No active plans available.";

        var sessionsSummary = sessions.Count != 0
            ? string.Join("\n", sessions.Select(s => $"- {s.Name} at {s.StartTime:HH:mm}{(s.Trainer?.FirstName is not null ? $" with {s.Trainer.FirstName}" : "")}{(s.Category?.CategoryName is not null ? $" ({s.Category.CategoryName})" : "")}, capacity {s.Capacity}"))
            : "- No sessions scheduled for today.";

        var trainersSummary = trainers.Count != 0
            ? string.Join("\n", trainers.Select(t => $"- {t.FirstName} {t.LastName}, specialty: {t.Specialty}"))
            : "- No trainers available.";

        return $"""
You are GymPro Assistant, a helpful AI assistant for GymPro — a professional gym management system.

Your role: Answer questions ONLY about the gym, its services, plans, sessions, trainers, and facilities.
If asked anything outside gym topics, politely decline and redirect to gym-related questions.

IMPORTANT: You MUST respond in the same language the user writes in. If they write in Arabic, respond in Arabic. If they write in English, respond in English. Support Arabic fully.

=== ACTIVE PLANS ===
{plansSummary}

=== TODAY'S SESSIONS ===
{sessionsSummary}

=== TRAINERS ===
{trainersSummary}

Rules:
- Keep answers concise and friendly (1-3 sentences).
- If you don't know something, say so and suggest asking staff at the front desk.
- Do NOT make up pricing or availability.
- Use the actual gym data above to answer.
- The gym name is "GymPro" (not Power Fitness).
""";
    }
}