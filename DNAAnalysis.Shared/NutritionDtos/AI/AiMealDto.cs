namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiMealDto
{
    public string MealType { get; set; } = null!;

    public int Calories { get; set; }

    public AiMacrosDto Macros { get; set; } = null!;

    public List<string> Options { get; set; }
        = new();
}