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
                .WithMessage("Drug1 is required.");

            RuleFor(x => x.Drug2)
                .NotEmpty()
                .WithMessage("Drug2 is required.");

            RuleFor(x => x)
                .Must(x => x.Drug1 != x.Drug2)
                .WithMessage("Drug1 and Drug2 cannot be the same.");
        }
    }
}