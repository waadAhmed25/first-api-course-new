using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities.NutritionModule;

public class UserMealSelection : BaseEntity<int>
{
    public string UserId { get; set; } = null!;

    public int MealSuggestionId { get; set; }

    public MealSuggestion MealSuggestion { get; set; } = null!;
}