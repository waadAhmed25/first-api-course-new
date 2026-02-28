using Microsoft.AspNetCore.Http;

namespace DNAAnalysis.Api.Requests;

public class CreateGeneticRequestFormDto
{
    public IFormFile? FatherFile { get; set; }
    public IFormFile? MotherFile { get; set; }
    public IFormFile? ChildFile { get; set; }
    public IFormFile? CombinedFile { get; set; }
}