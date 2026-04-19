using DNAAnalysis.Domain.Entities.IdentityModule;

namespace DNAAnalysis.Domain.Contracts;

public interface IOtpRepository
{
    Task AddAsync(OtpCode otp);

    Task<OtpCode?> GetValidOtpAsync(string userId, string code, string purpose);

    Task SaveChangesAsync();

    // ✅ جديد
    Task<List<OtpCode>> GetOtpsByUserAndPurposeAsync(string userId, string purpose);

    void RemoveRange(List<OtpCode> otps);
}