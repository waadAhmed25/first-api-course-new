using DNAAnalysis.Shared.DrugDtos;

namespace DNAAnalysis.Services.Abstraction;

public interface IDrugService
{
    Task<IEnumerable<DrugInteractionDto>> GetAllAsync();

    Task<DrugInteractionDto?> GetByIdAsync(int id, string userId, bool isAdmin);

    Task<IEnumerable<DrugInteractionDto>> GetUserDrugInteractionsAsync(string userId);

    Task<bool> DeleteInteractionAsync(int id, string userId, bool isAdmin);

    Task<DrugInteractionDto> CheckInteractionAsync(
        CheckDrugInteractionRequest request,
        string userId);

    Task<IEnumerable<DrugInteractionDto>> CheckMultipleInteractionsAsync(
    CheckMultipleDrugsRequest request,
    string userId);
}