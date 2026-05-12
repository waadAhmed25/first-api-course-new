using DNAAnalysis.Shared.NutritionDtos.AI;

namespace DNAAnalysis.Services.Abstraction;

public interface IAiNutritionClient
{
    Task<AiNutritionResponseDto> GeneratePlanAsync(
        AiNutritionRequestDto request);
}