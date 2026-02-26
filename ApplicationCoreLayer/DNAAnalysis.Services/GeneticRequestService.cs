using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.GeneticModule;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Services;

public class GeneticRequestService : IGeneticRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GeneticRequestService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateRequestAsync(string userId, CreateGeneticRequestDto dto)
    {
        var request = new GeneticRequest
        {
            UserId = userId,
            FatherFilePath = dto.FatherFilePath,
            MotherFilePath = dto.MotherFilePath,
            ChildFilePath = dto.ChildFilePath,
            CreatedAt = DateTime.UtcNow,
            Status = RequestStatus.Pending
        };

        await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .AddAsync(request);

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

    // ✅ دي اللي كانت ناقصة
    public async Task<GeneticRequestDto?> GetByIdForUserAsync(int id, string userId, bool isAdmin)
    {
        var request = await _unitOfWork
            .GetRepository<GeneticRequest, int>()
            .GetByIdAsync(id);

        if (request is null)
            return null;

        // لو مش Admin ومش صاحب الطلب
        if (!isAdmin && request.UserId != userId)
            return null;

        return _mapper.Map<GeneticRequestDto>(request);
    }

    public async Task UpdateStatusAsync(int id, RequestStatus status)
    {
        var repo = _unitOfWork.GetRepository<GeneticRequest, int>();

        var request = await repo.GetByIdAsync(id);

        if (request is null)
            throw new Exception("Request not found");

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;

        repo.Update(request);

        await _unitOfWork.SaveChangeAsync();
    }
}