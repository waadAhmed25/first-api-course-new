using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Shared.NutritionDtos;

public class MealSuggestionDto
{
    public int Id { get; set; }

    public MealType MealType { get; set; }

    public int Calories { get; set; }

    public double ProteinGrams { get; set; }

    public double CarbsGrams { get; set; }

    public double FatGrams { get; set; }

    public IEnumerable<string> Options { get; set; }
        = new List<string>();
}