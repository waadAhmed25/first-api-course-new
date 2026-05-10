using System.Text.Json;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.GeneticModule;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services
{
    public class GeneticResultService : IGeneticResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeneticResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneticResultDto?> GetResultByRequestIdAsync(int requestId)
        {
            var resultRepo = _unitOfWork.GetRepository<GeneticResult, int>();

            var result = await resultRepo
                .GetAsync(r => r.GeneticRequestId == requestId);

            if (result == null)
                return null;

            return new GeneticResultDto
            {
                RawAiResponse = JsonDocument
                    .Parse(result.RawAiResponse)
                    .RootElement
                    .Clone()
            };
        }
    }
}