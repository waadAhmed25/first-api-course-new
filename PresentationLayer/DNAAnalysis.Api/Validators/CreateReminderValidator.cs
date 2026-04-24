using DNAAnalysis.Shared.DTOs.Alarm;
using FluentValidation;

namespace DNAAnalysis.Api.Validators;

public class CreateReminderValidator : AbstractValidator<CreateReminderDto>
{
    public CreateReminderValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .MaximumLength(200)
            .Must(t => !string.IsNullOrWhiteSpace(t))
            .WithMessage("Title cannot be empty or spaces only");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(5).WithMessage("Description must be meaningful")
            .MaximumLength(500)
            .Must(d => !string.IsNullOrWhiteSpace(d))
            .WithMessage("Description cannot be empty or spaces only");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required (format: yyyy-MM-dd)")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Date must be today or later (format: yyyy-MM-dd)");

        RuleFor(x => x.StartTime)
            .NotNull()
            .WithMessage("Start time is required (format: HH:mm:ss)")
            .Must(t => t != TimeSpan.Zero)
            .WithMessage("Start time must be a valid time (format: HH:mm:ss)");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.EndTime.HasValue && x.StartTime.HasValue)
            .WithMessage("End time must be after start time (format: HH:mm:ss)");

        RuleFor(x => x.ReminderType)
            .IsInEnum()
            .WithMessage("Invalid reminder type");
    }
}