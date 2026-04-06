using DNAAnalysis.Shared.DrugDtos;
using FluentValidation;

namespace DNAAnalysis.Api.Validators;

public class CheckMultipleDrugsValidator : AbstractValidator<CheckMultipleDrugsRequest>
{
    public CheckMultipleDrugsValidator()
    {
        RuleFor(x => x.Drugs)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Drugs list is required")
            .Must(d => d != null && d.Count >= 2)
            .WithMessage("At least two drugs are required")
            .Must(d => d != null && d
                .Select(x => x.Trim().ToLower())
                .Distinct()
                .Count() == d.Count)
            .WithMessage("Duplicate drugs are not allowed");

        RuleForEach(x => x.Drugs)
            .Cascade(CascadeMode.Stop)
            .Must(d => !string.IsNullOrWhiteSpace(d))
            .WithMessage("Drug name cannot be empty")
            .MaximumLength(100);
    }
}