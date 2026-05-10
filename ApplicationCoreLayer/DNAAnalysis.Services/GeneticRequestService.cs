using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.GeneticModule;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace DNAAnalysis.Services;

public class GeneticRequestService : IGeneticRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGeneticAnalysisClient _aiClient;

    public GeneticRequestService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGeneticAnalysisClient aiClient)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _aiClient = aiClient;
    }

    public async Task<int> CreateRequestAsync(
        string userId,
        CreateGeneticRequestDto dto,
        IFormFile? fatherFile,
        IFormFile? motherFile,
        IFormFile? individualFile)
    {
        // ================= CREATE =================

        var request = new GeneticRequest
        {
            UserId = userId,

            // ✅ نحفظ paths عادي زي ما عندك
            FatherFilePath = dto.FatherFilePath,
            MotherFilePath = dto.MotherFilePath,
            ChildFilePath = dto.IndividualFilePath,

            TestType = dto.TestType,
            CreatedAt = DateTime.UtcNow,
            Status = RequestStatus.Processing
        };

        var requestRepo = _unitOfWork.GetRepository<GeneticRequest, int>();
        var resultRepo = _unitOfWork.GetRepository<GeneticResult, int>();

        await requestRepo.AddAsync(request);
        await _unitOfWork.SaveChangeAsync();

        try
        {
            // ✅ بقى نبعت files الحقيقية
            var rawJson = await _aiClient.AnalyzeAsync(
                fatherFile,
                motherFile,
                individualFile,
                request.TestType
            );

            // ✅ نخزن الخام فقط
            var geneticResult = new GeneticResult
            {
                GeneticRequestId = request.Id,
                RawAiResponse = rawJson
            };

            await resultRepo.AddAsync(geneticResult);

            request.Status = RequestStatus.Completed;
        }
        catch (Exception)
        {
            request.Status = RequestStatus.Failed;
        }

        await _unitOfWork.SaveChangeAsync();

        return request.Id;
    }

    public async Task<IEnumerable<GeneticRequestDto>> GetUserRequestsAsync(string userId)
    {
        var allRequests = await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .GetAllAsync();

        var userRequests = allRequests
            .Where(x => x.UserId == userId);

        return _mapper.Map<IEnumerable<GeneticRequestDto>>(userRequests);
    }

    public async Task<IEnumerable<GeneticRequestDto>> GetAllRequestsAsync()
    {
        var requests = await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .GetAllAsync();

        return _mapper.Map<IEnumerable<GeneticRequestDto>>(requests);
    }

    public async Task<GeneticRequestDto?> GetByIdAsync(int id)
    {
        var request = await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .GetByIdAsync(id);

        return _mapper.Map<GeneticRequestDto?>(request);
    }

    public async Task<GeneticRequestDto?> GetByIdForUserAsync(
        int id,
        string userId,
        bool isAdmin)
    {
        var request = await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .GetByIdAsync(id);

        if (request is null)
            return null;

        if (!isAdmin && request.UserId != userId)
            return null;

        return _mapper.Map<GeneticRequestDto>(request);
    }

    public async Task UpdateStatusAsync(int id, RequestStatus status)
    {
        var repo = _unitOfWork.GetRepository<GeneticRequest, int>();

        var request = await repo.GetByIdAsync(id);

        if (request is null)
            throw new ArgumentException("الطلب غير موجود");

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;

        repo.Update(request);

        await _unitOfWork.SaveChangeAsync();
    }
}