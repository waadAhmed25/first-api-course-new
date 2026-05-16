using DNAAnalysis.Domain.Entities.NutritionModule;
using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities;

public class UserMealSelection : BaseEntity<int>
{
    public string UserId { get; set; } = null!;

    public int MealOptionId { get; set; }

    public MealOption MealOption { get; set; } = null!;
}