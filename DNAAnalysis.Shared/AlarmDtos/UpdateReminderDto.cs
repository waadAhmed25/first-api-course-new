using DNAAnalysis.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace DNAAnalysis.Shared.DTOs.Alarm;

public class UpdateReminderDto
{
   [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public ReminderType? ReminderType { get; set; }   // 👈 fix

    [Required]
    public DateTime? Date { get; set; }               // 👈 fix

    public TimeSpan? StartTime { get; set; }          // 👈 خليها nullable

    public TimeSpan? EndTime { get; set; }

}
