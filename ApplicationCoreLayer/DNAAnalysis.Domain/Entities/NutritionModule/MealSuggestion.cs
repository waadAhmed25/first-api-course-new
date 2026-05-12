using DNAAnalysis.Domain.Entities;
using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Domain.Entities.NutritionModule;

public class MealSuggestion : BaseEntity<int>
{
    public int NutritionPlanId { get; set; }

    public MealType MealType { get; set; }

    public int Calories { get; set; }

    // ✅ AI Macros
    public double ProteinGrams { get; set; }

    public double CarbsGrams { get; set; }

    public double FatGrams { get; set; }

    public NutritionPlan NutritionPlan { get; set; } = null!;

    // ✅ Meal Options
    public ICollection<MealOption> Options { get; set; }
        = new List<MealOption>();
}