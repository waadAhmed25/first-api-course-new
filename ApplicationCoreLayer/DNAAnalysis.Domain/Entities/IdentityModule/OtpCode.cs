using System;

namespace DNAAnalysis.Domain.Entities.IdentityModule;



public class OtpCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;   // 5 digits

    public string UserId { get; set; } = null!;

    public string Purpose { get; set; } = null!; 
    // EmailConfirmation
    // PasswordReset

    public DateTime ExpirationTime { get; set; }

    public bool IsUsed { get; set; } = false;

    // Navigation Property
    public ApplicationUser User { get; set; } = null!;
}

