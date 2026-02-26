using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Shared.Enums;

public interface IGeneticRequestService
{
    Task<int> CreateRequestAsync(string userId, CreateGeneticRequestDto dto);

    Task<IEnumerable<GeneticRequestDto>> GetUserRequestsAsync(string userId);

    Task<IEnumerable<GeneticRequestDto>> GetAllRequestsAsync();

    Task<GeneticRequestDto?> GetByIdAsync(int id);

    // ✅ الجديدة
    Task<GeneticRequestDto?> GetByIdForUserAsync(int id, string userId, bool isAdmin);

    Task UpdateStatusAsync(int id, RequestStatus status);
}