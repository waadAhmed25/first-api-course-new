using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.DrugDtos;
using DNAAnalysis.Domain.Entities.DrugModule;

namespace DNAAnalysis.Services;

public class DrugService : IDrugService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDrugInteractionClient _drugClient;

    public DrugService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDrugInteractionClient drugClient)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _drugClient = drugClient;
    }

    // ================= ADMIN ONLY =================
    public async Task<IEnumerable<DrugInteractionDto>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<DrugInteraction, int>();
        var drugs = await repo.GetAllAsync();
        return _mapper.Map<IEnumerable<DrugInteractionDto>>(drugs);
    }

    // ================= GET BY ID (SECURE) =================
    public async Task<DrugInteractionDto?> GetByIdAsync(int id, string userId, bool isAdmin)
    {
        var repo = _unitOfWork.GetRepository<DrugInteraction, int>();
        var drug = await repo.GetByIdAsync(id);

        if (drug is null)
            return null;

        if (!isAdmin && drug.UserId != userId)
            return null;

        return _mapper.Map<DrugInteractionDto>(drug);
    }

    // ================= USER HISTORY =================
    public async Task<IEnumerable<DrugInteractionDto>> GetUserDrugInteractionsAsync(string userId)
    {
        var repo = _unitOfWork.GetRepository<DrugInteraction, int>();

        var userInteractions =
            await repo.GetAllAsync(d => d.UserId == userId);

        return _mapper.Map<IEnumerable<DrugInteractionDto>>(userInteractions);
    }

    // ================= DELETE =================
    public async Task<bool> DeleteInteractionAsync(int id, string userId, bool isAdmin)
    {
        var repo = _unitOfWork.GetRepository<DrugInteraction, int>();
        var interaction = await repo.GetByIdAsync(id);

        if (interaction is null)
            return false;

        if (!isAdmin && interaction.UserId != userId)
            return false;

        repo.Remove(interaction);
        await _unitOfWork.SaveChangeAsync();

        return true;
    }

    // ================= CHECK INTERACTION =================
    public async Task<DrugInteractionDto> CheckInteractionAsync(
        CheckDrugInteractionRequest request,
        string userId)
    {
        // ✅ Normalize فقط
        request.Drug1 = request.Drug1.Trim();
        request.Drug2 = request.Drug2.Trim();

        var aiResult = await _drugClient.CheckInteractionAsync(request);

        aiResult.UserId = userId;

        var repo = _unitOfWork.GetRepository<DrugInteraction, int>();
        var entity = _mapper.Map<DrugInteraction>(aiResult);

        await repo.AddAsync(entity);
        await _unitOfWork.SaveChangeAsync();

        return aiResult;
    }

    // ================= CHECK MULTIPLE =================
    public async Task<IEnumerable<DrugInteractionDto>> CheckMultipleInteractionsAsync(
        CheckMultipleDrugsRequest request,
        string userId)
    {
        // ✅ Prevent null (بدون ما نكرر validation)
        var drugs = request.Drugs ?? new List<string>();

        // ✅ Normalize + Clean
        drugs = drugs
            .Select(d => d.Trim())
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var results = new List<DrugInteractionDto>();

        // loop على كل combinations
        for (int i = 0; i < drugs.Count; i++)
        {
            for (int j = i + 1; j < drugs.Count; j++)
            {
                var singleRequest = new CheckDrugInteractionRequest
                {
                    Drug1 = drugs[i],
                    Drug2 = drugs[j]
                };

                var result = await CheckInteractionAsync(singleRequest, userId);

                results.Add(result);
            }
        }

        return results;
    }
}