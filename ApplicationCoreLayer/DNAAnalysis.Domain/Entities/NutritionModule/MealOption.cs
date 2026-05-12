using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities.NutritionModule;

public class MealOption : BaseEntity<int>
{
    public int MealSuggestionId { get; set; }

    public string Name { get; set; } = null!;

    public MealSuggestion MealSuggestion { get; set; } = null!;
}