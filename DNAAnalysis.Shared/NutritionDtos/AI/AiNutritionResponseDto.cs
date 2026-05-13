using System.Text.Json.Serialization;

namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiNutritionResponseDto
{
    [JsonPropertyName("bmr")]
    public double Bmr { get; set; }

    [JsonPropertyName("tdee")]
    public double Tdee { get; set; }

    [JsonPropertyName("final_calories_goal")]
    public double FinalCaloriesGoal { get; set; }

    [JsonPropertyName("meal_plan")]
    public Dictionary<string, AiMealDto> MealPlan { get; set; }
        = new();
}