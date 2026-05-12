namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiNutritionRequestDto
{
    public double Weight { get; set; }

    public double Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public string ActivityLevel { get; set; } = null!;

    public string HealthCondition { get; set; } = null!;
}