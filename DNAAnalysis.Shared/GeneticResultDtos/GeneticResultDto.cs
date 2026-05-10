using System.Text.Json;

namespace DNAAnalysis.Shared.GeneticResultDtos;

public class GeneticResultDto
{
    public JsonElement RawAiResponse { get; set; }
}