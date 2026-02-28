using DNAAnalysis.Shared.GeneticResultDtos;

public interface IGeneticAnalysisClient
{
    Task<GeneticResultDto> AnalyzeAsync(
        string fatherPath,
        string motherPath,
        string? childPath);
}