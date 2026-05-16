using DNAAnalysis.Shared.NutritionDtos;
using FluentValidation;

namespace DNAAnalysis.Api.Validators;

public class CreateNutritionProfileValidator : AbstractValidator<CreateNutritionProfileDto>
{
    public CreateNutritionProfileValidator()
    {
        RuleFor(x => x.Weight)
            .NotEmpty().WithMessage("Weight is required")
            .GreaterThan(0).WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Weight must not exceed 500 kg");

        RuleFor(x => x.Height)
            .NotEmpty().WithMessage("Height is required")
            .GreaterThan(50).WithMessage("Height must be greater than 50 cm")
            .LessThanOrEqualTo(300).WithMessage("Height must not exceed 300 cm");

        RuleFor(x => x.Age)
            .NotEmpty().WithMessage("Age is required")
            .GreaterThan(0).WithMessage("Age must be greater than 0")
            .LessThanOrEqualTo(120).WithMessage("Age must not exceed 120");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(x => x.ToLower() == "male" || x.ToLower() == "female")
            .WithMessage("Gender must be male or female");

        RuleFor(x => x.Activity)
            .NotEmpty().WithMessage("Activity level is required")
            .Must(x =>
                x.ToLower() == "sedentary" ||
                x.ToLower() == "lightly active" ||
                x.ToLower() == "moderately active" ||
                x.ToLower() == "very active")
            .WithMessage("Invalid activity level");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Health status is required")
            .Must(x =>
                x.ToLower() == "healthy" ||
                x.ToLower() == "diabetic" ||
                x.ToLower() == "hypertension")
            .WithMessage("Invalid health status");
    }
}