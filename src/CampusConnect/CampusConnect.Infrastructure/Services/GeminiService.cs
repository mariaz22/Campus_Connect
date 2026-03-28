using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusConnect.Application.DTOs.Assistant;
using CampusConnect.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CampusConnect.Infrastructure.Services;

public class GeminiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly JsonSerializerOptions _jsonOptions;

    public GeminiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured");
        _model = configuration["Gemini:Model"] ?? "gemini-2.0-flash-lite";

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ChatResponse> GenerateResponseAsync(string userMessage, UserContextDto userContext, string? sessionId = null)
    {
        var systemPrompt = BuildSystemPrompt(userContext);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"{systemPrompt}\n\nUser: {userMessage}" }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 500
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody, _jsonOptions),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsync(url, jsonContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Gemini API error: {response.StatusCode} - {responseContent}");
                return new ChatResponse
                {
                    Message = $"API Error: {response.StatusCode}. Please check the console for details.",
                    SessionId = sessionId ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                };
            }

            var responseJson = JsonSerializer.Deserialize<GeminiResponse>(responseContent, _jsonOptions);
            var assistantMessage = responseJson?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                ?? "I'm sorry, I couldn't generate a response.";

            var suggestedActions = ExtractSuggestedActions(assistantMessage, userMessage);

            return new ChatResponse
            {
                Message = CleanMessage(assistantMessage),
                SessionId = sessionId ?? Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                SuggestedActions = suggestedActions
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API exception: {ex.Message}");
            return new ChatResponse
            {
                Message = "I'm having trouble connecting to my AI service right now. Please try again later.",
                SessionId = sessionId ?? Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private string BuildSystemPrompt(UserContextDto userContext)
    {
        var groupsInfo = userContext.GroupNames.Count > 0
            ? $"The user is a member of these groups: {string.Join(", ", userContext.GroupNames)}."
            : "The user is not currently a member of any groups.";

        var subjectsInfo = userContext.SubjectNames.Count > 0
            ? $"The user is enrolled in these subjects: {string.Join(", ", userContext.SubjectNames)}."
            : "";

        return $@"You are a helpful campus assistant for CampusConnect, the student portal for the University of Bucharest (Universitatea din București / Unibuc).

Current user information:
- Name: {userContext.FirstName} {userContext.LastName}
- Role: {userContext.Role}
- Email: {userContext.Email}
{groupsInfo}
{subjectsInfo}

Campus information:
- Total buildings: {userContext.TotalBuildings} faculty buildings
- Total rooms: {userContext.TotalRooms} rooms available

Available features in CampusConnect:
1. Groups - Join or create study groups, view group tasks and materials
2. Tasks - View and manage homework and assignments
3. Events - Browse and join campus events
4. Campus Map - Find buildings and rooms on campus
5. Library - Access course materials and documents
6. Grades - View grades and academic progress
7. Announcements - Stay updated with campus news
8. Profile - Manage account settings

Guidelines:
- Be friendly, helpful, and concise
- Address the user by their first name when appropriate
- When users ask about navigating to a feature, mention the relevant page
- Keep responses focused and under 150 words when possible
- Use Romanian language if the user writes in Romanian, otherwise respond in English";
    }

    private List<SuggestedAction>? ExtractSuggestedActions(string message, string userQuestion)
    {
        var actions = new List<SuggestedAction>();
        var lowerMessage = message.ToLowerInvariant();
        var lowerQuestion = userQuestion.ToLowerInvariant();

        if (lowerMessage.Contains("group") || lowerQuestion.Contains("group") || lowerQuestion.Contains("grup"))
            actions.Add(new SuggestedAction { Label = "Browse Groups", ActionType = "navigate", Payload = "/groups" });

        if (lowerMessage.Contains("task") || lowerQuestion.Contains("task") || lowerQuestion.Contains("homework"))
            actions.Add(new SuggestedAction { Label = "View Tasks", ActionType = "navigate", Payload = "/my-tasks" });

        if (lowerMessage.Contains("event") || lowerQuestion.Contains("event") || lowerQuestion.Contains("eveniment"))
            actions.Add(new SuggestedAction { Label = "View Events", ActionType = "navigate", Payload = "/events" });

        if (lowerMessage.Contains("map") || lowerMessage.Contains("building") || lowerMessage.Contains("room") ||
            lowerQuestion.Contains("map") || lowerQuestion.Contains("building") || lowerQuestion.Contains("room"))
            actions.Add(new SuggestedAction { Label = "Open Campus Map", ActionType = "navigate", Payload = "/campus-map" });

        if (lowerMessage.Contains("grade") || lowerQuestion.Contains("grade") || lowerQuestion.Contains("note"))
            actions.Add(new SuggestedAction { Label = "View Grades", ActionType = "navigate", Payload = "/my-grades" });

        if (lowerMessage.Contains("library") || lowerQuestion.Contains("library") || lowerQuestion.Contains("material"))
            actions.Add(new SuggestedAction { Label = "Open Library", ActionType = "navigate", Payload = "/library" });

        if (lowerMessage.Contains("announcement") || lowerQuestion.Contains("announcement") || lowerQuestion.Contains("news"))
            actions.Add(new SuggestedAction { Label = "View Announcements", ActionType = "navigate", Payload = "/announcements" });

        return actions.Count > 0 ? actions.Take(3).ToList() : null;
    }

    private string CleanMessage(string message) => message.Trim();
}

// Gemini API response models
internal class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

internal class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }
}

internal class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart>? Parts { get; set; }
}

internal class GeminiPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
