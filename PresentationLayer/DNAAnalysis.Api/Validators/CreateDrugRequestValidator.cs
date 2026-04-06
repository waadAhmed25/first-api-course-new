using FluentValidation;
using DNAAnalysis.Shared.DrugDtos;

namespace DNAAnalysis.API.Validators
{
    public class CheckDrugInteractionRequestValidator 
        : AbstractValidator<CheckDrugInteractionRequest>
    {
        public CheckDrugInteractionRequestValidator()
        {
            RuleFor(x => x.Drug1)
                .NotEmpty()
                .WithMessage("Drug1 is required.")
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Drug1 cannot be empty or whitespace.");

            RuleFor(x => x.Drug2)
                .NotEmpty()
                .WithMessage("Drug2 is required.")
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Drug2 cannot be empty or whitespace.");

            RuleFor(x => x)
                .Must(x => 
                    !string.Equals(
                        x.Drug1?.Trim(), 
                        x.Drug2?.Trim(), 
                        StringComparison.OrdinalIgnoreCase))
                .WithMessage("Drug1 and Drug2 cannot be the same.");
        }
    }
}