using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities.NutritionModule;

public class NutritionPlan : BaseEntity<int>
{
    public int NutritionProfileId { get; set; }

    // ✅ AI Main Values
    public double Bmr { get; set; }

    public double Tdee { get; set; }

    public double FinalCaloriesGoal { get; set; }

    // ✅ نخزن الـ response الخام كامل
    public string AiRawResponse { get; set; } = null!;

    public NutritionProfile NutritionProfile { get; set; } = null!;

    public ICollection<MealSuggestion> MealSuggestions { get; set; }
        = new List<MealSuggestion>();
}