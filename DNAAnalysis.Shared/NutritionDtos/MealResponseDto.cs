using DNAAnalysis.Shared.NutritionDtos.AI;

namespace DNAAnalysis.Shared.NutritionDtos;

public class MealResponseDto
{
    public int Calories { get; set; }

    public AiMacrosDto Macros { get; set; } = null!;

    public List<MealOptionDto> Options { get; set; } = new();
}