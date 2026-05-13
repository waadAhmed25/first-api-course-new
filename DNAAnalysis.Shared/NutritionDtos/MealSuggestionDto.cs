using DNAAnalysis.Shared.Enums;
using System.Text.Json.Serialization;

namespace DNAAnalysis.Shared.NutritionDtos;

public class MealSuggestionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("meal_type")]
    public MealType MealType { get; set; }

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("protein_grams")]
    public double ProteinGrams { get; set; }

    [JsonPropertyName("carbs_grams")]
    public double CarbsGrams { get; set; }

    [JsonPropertyName("fat_grams")]
    public double FatGrams { get; set; }

    [JsonPropertyName("options")]
    public IEnumerable<string> Options { get; set; }
        = new List<string>();
}