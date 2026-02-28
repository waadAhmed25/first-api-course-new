using AutoMapper;
using DNAAnalysis.Shared.DrugDtos;
using DNAAnalysis.Domain.Entities.DrugModule;

namespace DNAAnalysis.Services.MappingProfiles;

public class DrugProfile : Profile
{
    public DrugProfile()
    {
        CreateMap<DrugInteraction, DrugInteractionDto>().ReverseMap();
    }
}