using DNAAnalysis.Shared.NutritionDtos;

namespace DNAAnalysis.ServiceAbstraction;

public interface INutritionService
{
    
    Task CreateProfileAsync(string userId, CreateNutritionProfileDto dto);

    Task<NutritionPlanDto?> GeneratePlanAsync(string userId);

    Task<NutritionPlanDto?> GetUserPlanAsync(string userId);

    Task SelectMealAsync(string userId, int mealId);

    Task UnselectMealAsync(string userId, int mealId);
}