using System.Net.Http.Json;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.NutritionDtos;
using DNAAnalysis.Shared.NutritionDtos.AI;

namespace DNAAnalysis.Services;

public class RealAiNutritionClient : IAiNutritionClient
{
    private readonly HttpClient _httpClient;

    public RealAiNutritionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

public async Task<AiNutritionResponseDto> GeneratePlanAsync(            AiNutritionRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/generate-plan",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"AI Nutrition API Error: {error}");
        }

        var result =
await response.Content.ReadFromJsonAsync<AiNutritionResponseDto>();
        if (result == null)
            throw new Exception("AI response was null");

        return result;
    }
}