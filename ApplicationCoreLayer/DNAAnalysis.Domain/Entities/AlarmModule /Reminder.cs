using DNAAnalysis.Domain;
using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Domain.Entities.AlarmModule;

public class Reminder : BaseEntity<int>
{
    public string UserId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ReminderType ReminderType { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan Time { get; set; }

    public bool IsCompleted { get; set; } = false;

    public bool IsDeleted { get; set; }
}
