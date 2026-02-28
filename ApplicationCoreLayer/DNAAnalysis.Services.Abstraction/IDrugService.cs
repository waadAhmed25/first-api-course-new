using DNAAnalysis.Shared.DrugDtos;

namespace DNAAnalysis.Services.Abstraction;

public interface IDrugService
{
    Task<IEnumerable<DrugInteractionDto>> GetAllAsync();

    Task<DrugInteractionDto?> GetByIdAsync(int id);

    Task AddAsync(DrugInteractionDto dto);
    Task<IEnumerable<DrugInteractionDto>> GetUserDrugInteractionsAsync(string userId);
Task<bool> DeleteInteractionAsync(int id, string userId);
}
