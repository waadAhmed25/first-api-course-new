using AutoMapper;
using DNAAnalysis.Domain.Entities;
using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Domain.Entities.GeneticModule;
namespace DNAAnalysis.Services.MappingProfiles;



public class GeneticRequestProfile : Profile
{
    public GeneticRequestProfile()
    {
        CreateMap<GeneticRequest, GeneticRequestDto>();
    }
}