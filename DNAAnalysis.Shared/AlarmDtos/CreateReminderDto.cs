using DNAAnalysis.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace DNAAnalysis.Shared.DTOs.Alarm;

public class CreateReminderDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public ReminderType? ReminderType { get; set; }   // 👈 مهم

    [Required]
    public DateTime? Date { get; set; }  

    
    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }
}