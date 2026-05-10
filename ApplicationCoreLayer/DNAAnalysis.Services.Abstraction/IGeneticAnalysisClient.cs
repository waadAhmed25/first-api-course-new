using Microsoft.AspNetCore.Http;
using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Services.Abstraction;

public interface IGeneticAnalysisClient
{
    Task<string> AnalyzeAsync(
        IFormFile? fatherFile,
        IFormFile? motherFile,
        IFormFile? individualFile,
        TestType testType);
}