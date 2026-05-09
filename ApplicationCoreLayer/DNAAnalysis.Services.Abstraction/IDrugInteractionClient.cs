using DNAAnalysis.Shared.DrugDtos;

namespace DNAAnalysis.Services.Abstraction;

public interface IDrugInteractionClient
{
    Task<DrugInteractionDto> CheckInteractionAsync(CheckDrugInteractionRequest request);

    Task<object> CheckMultipleInteractionsAsync(List<string> drugs);
}