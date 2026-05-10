using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Shared.Enums;
using Microsoft.AspNetCore.Http;

public interface IGeneticRequestService
{
    Task<int> CreateRequestAsync(
        string userId,
        CreateGeneticRequestDto dto,
        IFormFile? fatherFile,
        IFormFile? motherFile,
        IFormFile? individualFile);

    Task<IEnumerable<GeneticRequestDto>> GetUserRequestsAsync(string userId);

    Task<IEnumerable<GeneticRequestDto>> GetAllRequestsAsync();

    Task<GeneticRequestDto?> GetByIdAsync(int id);

    Task<GeneticRequestDto?> GetByIdForUserAsync(
        int id,
        string userId,
        bool isAdmin);

    Task UpdateStatusAsync(int id, RequestStatus status);
}