using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services.Abstraction;

public interface IGeneticAnalysisClient
{
    Task<GeneticResultDto> AnalyzeAsync(
        string fatherPath,
        string motherPath,
        string? childPath);
}