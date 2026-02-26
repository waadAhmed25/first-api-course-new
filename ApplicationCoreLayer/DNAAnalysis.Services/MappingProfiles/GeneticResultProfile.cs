using AutoMapper;
using DNAAnalysis.Domain.Entities.GeneticModule;
using DNAAnalysis.Shared.GeneticResultDtos;

namespace DNAAnalysis.Services.MappingProfiles;

public class GeneticResultProfile : Profile
{
    public GeneticResultProfile()
    {
        CreateMap<CreateGeneticResultDto, GeneticResult>();
        CreateMap<GeneticResult, GeneticResultDto>();
    }
}