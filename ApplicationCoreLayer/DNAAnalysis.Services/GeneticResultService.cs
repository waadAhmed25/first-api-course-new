using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.GeneticModule;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.Enums;
using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services
{
    public class GeneticResultService : IGeneticResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GeneticResultService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddResultAsync(int requestId, CreateGeneticResultDto dto)
        {
            var requestRepo = _unitOfWork.GetRepository<GeneticRequest, int>();
            var resultRepo  = _unitOfWork.GetRepository<GeneticResult, int>();

            var request = await requestRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new Exception("Request not found");

            var existingResult = await resultRepo
                .GetAsync(r => r.GeneticRequestId == requestId);

            if (existingResult != null)
                throw new Exception("Result already exists");

            var result = _mapper.Map<GeneticResult>(dto);
            result.GeneticRequestId = requestId;

            await resultRepo.AddAsync(result);

            // 👇 نغير الحالة حسب الـ enum عندك
            request.Status = RequestStatus.Completed;

            requestRepo.Update(request);

            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<GeneticResultDto?> GetResultByRequestIdAsync(int requestId)
        {
            var resultRepo = _unitOfWork.GetRepository<GeneticResult, int>();

            var result = await resultRepo
                .GetAsync(r => r.GeneticRequestId == requestId);

            if (result == null)
                return null;

            return _mapper.Map<GeneticResultDto>(result);
        }
    }
}