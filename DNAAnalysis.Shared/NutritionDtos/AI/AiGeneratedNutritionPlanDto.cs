namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiGeneratedNutritionPlanDto
{
    public double Bmr { get; set; }

    public double Tdee { get; set; }

    public double FinalCaloriesGoal { get; set; }

    public IEnumerable<MealSuggestionDto> Meals { get; set; }
        = new List<MealSuggestionDto>();
}