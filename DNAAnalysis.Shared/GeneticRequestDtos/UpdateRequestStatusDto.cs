using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Shared.GeneticRequestDtos;

public class UpdateRequestStatusDto
{
     /// <summary>
    /// Allowed values: Pending, Processing, Completed, Failed
    /// </summary>
    public RequestStatus Status { get; set; }
}
