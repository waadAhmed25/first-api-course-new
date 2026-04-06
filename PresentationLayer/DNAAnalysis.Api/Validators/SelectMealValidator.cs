using FluentValidation;
using DNAAnalysis.Shared.NutritionDtos;

namespace DNAAnalysis.Api.Validators;

public class SelectMealRequestValidator : AbstractValidator<SelectMealRequest>
{
    public SelectMealRequestValidator()
    {
        RuleFor(x => x.MealId)
            .GreaterThan(0)
            .WithMessage("Meal id must be greater than 0");
    }
}