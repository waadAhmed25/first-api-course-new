using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.DrugDtos;

namespace DNAAnalysis.Services;

public class RealDrugInteractionClient : IDrugInteractionClient
{
    private readonly HttpClient _httpClient;

    public RealDrugInteractionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DrugInteractionDto> CheckInteractionAsync(CheckDrugInteractionRequest request)
    {
        var aiRequest = new
        {
            drugs = new[] { request.Drug1, request.Drug2 }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(aiRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("/check", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("AI service failed");

        var json = await response.Content.ReadAsStringAsync();

        var aiResponse = JsonSerializer.Deserialize<AIResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var conflict = aiResponse?.Conflicts?.FirstOrDefault();

        return new DrugInteractionDto
        {
            Drug1 = request.Drug1,
            Drug2 = request.Drug2,
            HasInteraction = aiResponse?.HasInteraction ?? false,
            Severity = conflict?.Severity,
            Description = aiResponse?.Message
        };
    }


public async Task<object> CheckMultipleInteractionsAsync(List<string> drugs)
{
    var aiRequest = new
    {
        drugs = drugs
    };

    var content = new StringContent(
        JsonSerializer.Serialize(aiRequest),
        Encoding.UTF8,
        "application/json"
    );

    var response = await _httpClient.PostAsync("/check", content);

    if (!response.IsSuccessStatusCode)
        throw new Exception("AI service failed");

    var json = await response.Content.ReadAsStringAsync();

    return JsonSerializer.Deserialize<object>(
        json,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
}
    // ================= AI DTO =================
    private class AIResponse
    {
        [JsonPropertyName("has_interaction")]
        public bool HasInteraction { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("conflicts")]
        public List<Conflict>? Conflicts { get; set; }
    }

    private class Conflict
    {
        [JsonPropertyName("drug1")]
        public string? Drug1 { get; set; }

        [JsonPropertyName("drug2")]
        public string? Drug2 { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }
    }
}