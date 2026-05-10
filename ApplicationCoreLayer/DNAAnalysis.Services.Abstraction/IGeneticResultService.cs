using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services.Abstraction
{
    public interface IGeneticResultService
    {

        Task<GeneticResultDto?> GetResultByRequestIdAsync(int requestId);
    }
}