using System.Text.Json.Serialization;

namespace DNAAnalysis.Shared.NutritionDtos;

public class NutritionPlanDto
{
    [JsonPropertyName("bmr")]
    public double Bmr { get; set; }

    [JsonPropertyName("tdee")]
    public double Tdee { get; set; }

    [JsonPropertyName("final_calories_goal")]
    public double FinalCaloriesGoal { get; set; }

    [JsonPropertyName("eaten_calories")]
    public int EatenCalories { get; set; }

    [JsonPropertyName("remaining_calories")]
    public double RemainingCalories { get; set; }

    [JsonPropertyName("meal_plan")]
    public IEnumerable<MealSuggestionDto> Meals { get; set; }
        = new List<MealSuggestionDto>();
}