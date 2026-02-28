using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services;

public class FakeGeneticAnalysisClient : IGeneticAnalysisClient
{
    public async Task<GeneticResultDto> AnalyzeAsync(
        string fatherPath,
        string motherPath,
        string? childPath)
    {
        await Task.Delay(2000);

        return new GeneticResultDto
        {
            FatherStatus = "Compatible",
            MotherStatus = "Carrier",
            ChildStatus = "Healthy",
            MessageToPatient = "No critical mutation detected.",
            Advice = "Regular yearly follow-up recommended."
        };
    }
}