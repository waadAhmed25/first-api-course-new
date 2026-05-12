namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiNutritionResponseDto
{
    public double Bmr { get; set; }

    public double Tdee { get; set; }

    public double FinalCaloriesGoal { get; set; }

    public List<AiMealDto> MealPlan { get; set; }
        = new();
}