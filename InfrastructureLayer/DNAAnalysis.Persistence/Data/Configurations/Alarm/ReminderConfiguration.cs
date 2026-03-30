using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DNAAnalysis.Domain.Entities.AlarmModule;


namespace DNAAnalysis.Persistence.Data.Configurations.Alarm;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(r => r.Description)
               .HasMaxLength(500);

        builder.Property(r => r.UserId)
               .IsRequired();

        builder.Property(r => r.Date)
               .IsRequired();

        builder.Property(r => r.Time)
               .IsRequired();

        builder.Property(r => r.ReminderType)
               .IsRequired();

        builder.Property(r => r.IsCompleted)
               .HasDefaultValue(false);

        builder.Property(r => r.IsDeleted)
               .HasDefaultValue(false);

        // 🔥 أهم حاجة (زي ما عملنا تحسين في Drug)
        builder.HasIndex(r => new { r.UserId, r.Date });
    }
}