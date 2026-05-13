using DNAAnalysis.Shared.NutritionDtos;
using FluentValidation;

namespace DNAAnalysis.Api.Validators;

public class CreateNutritionProfileValidator : AbstractValidator<CreateNutritionProfileDto>
{
    public CreateNutritionProfileValidator()
    {
        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .LessThanOrEqualTo(500)
            .WithMessage("Weight must be between 1 and 500 kg");

        RuleFor(x => x.Height)
            .GreaterThan(50)
            .LessThanOrEqualTo(300)
            .WithMessage("Height must be between 50 and 300 cm");

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .LessThanOrEqualTo(120)
            .WithMessage("Age must be between 1 and 120");

        RuleFor(x => x.Gender)
    .NotEmpty();

RuleFor(x => x.Activity)
    .NotEmpty();

RuleFor(x => x.Status)
    .NotEmpty();
    }
}