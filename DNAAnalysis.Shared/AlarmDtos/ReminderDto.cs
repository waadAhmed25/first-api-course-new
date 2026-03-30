using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Shared.DTOs.Alarm;

public class ReminderDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ReminderType ReminderType { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan Time { get; set; }

    public bool IsCompleted { get; set; }
}