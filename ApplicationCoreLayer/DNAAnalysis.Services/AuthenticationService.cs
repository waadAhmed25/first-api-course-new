using DNAAnalysis.Domain.Entities.IdentityModule;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.CommonResult;
using DNAAnalysis.Shared.IdentityDtos;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using DNAAnalysis.Domain.Contracts;
using Microsoft.EntityFrameworkCore;



namespace DNAAnalysis.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailService _emailService;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IOtpRepository otpRepository,
        IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _otpRepository = otpRepository;
        _emailService = emailService;
    }

    // ================= LOGIN =================
    public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
            return Error.InvalidCredentials("User.InvalidCred");

        if (!user.EmailConfirmed)
            return Error.Validation("Email.NotConfirmed", "Please confirm your email first");

        var isPasswordValid =
            await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordValid)
            return Error.InvalidCredentials("User.InvalidCred");

        var token = await CreateTokenAsync(user);
        return new UserDto(user.Email!, user.DisplayName, token);
    }

    // ================= REGISTER =================
    public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto)
{
    // ✅ check لو الإيميل موجود قبل ما نعمل create
    var users = await _userManager.Users
        .Where(u => u.Email == registerDto.Email)
        .ToListAsync();

    var existingUser = users.FirstOrDefault();

    if (users.Count > 1)
    {
        Console.WriteLine("WARNING: Duplicate emails detected in Register.");
    }

    if (existingUser != null)
    {
        // ✅ check لو Admin
        var roles = await _userManager.GetRolesAsync(existingUser);

        if (roles.Contains("Admin"))
        {
            return Error.Validation("Email.AdminExists", "This email belongs to an admin account");
        }

        return Error.Validation("Email.AlreadyExists", "Email is already registered");
    }

    // ✅ create user طبيعي
    var user = new ApplicationUser
    {
        Email = registerDto.Email,
        DisplayName = registerDto.DisplayName,
        PhoneNumber = registerDto.PhoneNumber,
        UserName = registerDto.UserName
    };

    var identityResult =
        await _userManager.CreateAsync(user, registerDto.Password);

    if (!identityResult.Succeeded)
    {
        return identityResult.Errors
            .Select(E => Error.Validation(E.Code, E.Description))
            .ToList();
    }

    var otp = new OtpCode
    {
        Code = new Random().Next(10000, 99999).ToString(),
        UserId = user.Id,
        Purpose = "EmailConfirmation",
        ExpirationTime = DateTime.UtcNow.AddMinutes(10),
        IsUsed = false
    };

    await _otpRepository.AddAsync(otp);
    await _otpRepository.SaveChangesAsync();

    try
    {
        await _emailService.SendEmailAsync(
            user.Email!,
            "Email Confirmation OTP",
            $"Your OTP Code is: {otp.Code}"
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Email send failed: {ex.Message}");
    }

    return new UserDto(user.Email!, user.DisplayName, string.Empty);
}
    // ================= CONFIRM EMAIL =================
    public async Task<Result<bool>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
{
    var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);

    if (user is null)
        return Error.NotFound();

    var otp = await _otpRepository
        .GetValidOtpAsync(user.Id, confirmEmailDto.OtpCode, "EmailConfirmation");

    if (otp is null)
        return Error.Validation("OTP.Invalid", "Invalid or expired OTP");

    user.EmailConfirmed = true;

    var updateResult = await _userManager.UpdateAsync(user);

    if (!updateResult.Succeeded)
    {
        return updateResult.Errors
            .Select(e => Error.Validation(e.Code, e.Description))
            .ToList();
    }

    otp.IsUsed = true;
    await _otpRepository.SaveChangesAsync();

    return true;
}

    // ================= RESEND CONFIRMATION =================
  public async Task<Result<bool>> ResendConfirmationAsync(ResendConfirmationDto dto)
{
    var users = await _userManager.Users
        .Where(u => u.Email == dto.Email)
        .ToListAsync();

    var user = users.FirstOrDefault();

    if (user is null)
        return Error.NotFound();

    if (users.Count > 1)
    {
        Console.WriteLine("WARNING: Duplicate emails detected.");
    }

    if (user.EmailConfirmed)
        return Error.Validation("Email.AlreadyConfirmed", "Email already confirmed");

    var otp = new OtpCode
    {
        Code = Guid.NewGuid().ToString("N").Substring(0, 5),
        UserId = user.Id,
        Purpose = "EmailConfirmation",
        ExpirationTime = DateTime.UtcNow.AddMinutes(10),
        IsUsed = false
    };

    try
    {
        var oldOtps = await _otpRepository
            .GetOtpsByUserAndPurposeAsync(user.Id, "EmailConfirmation");

        if (oldOtps.Any())
            _otpRepository.RemoveRange(oldOtps);

        await _otpRepository.AddAsync(otp);
        await _otpRepository.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email!,
            "Resend Email Confirmation OTP",
            $"Your OTP Code is: {otp.Code}"
        );

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"RESEND ERROR FULL: {ex}");
        return Error.Validation("Server.Error", ex.Message);
    }
}
    // ================= FORGOT PASSWORD =================
    public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
{
    var users = await _userManager.Users
        .Where(u => u.Email == forgotPasswordDto.Email)
        .ToListAsync();

    var user = users.FirstOrDefault();

    if (users.Count > 1)
    {
        Console.WriteLine("WARNING: Duplicate emails detected in ForgotPassword.");
    }

    if (user is null)
        return Error.Validation("Email.NotFound", "Email is not registered");

    var otp = new OtpCode
    {
        Code = new Random().Next(10000, 99999).ToString(),
        UserId = user.Id,
        Purpose = "PasswordReset",
        ExpirationTime = DateTime.UtcNow.AddMinutes(10),
        IsUsed = false
    };

    await _otpRepository.AddAsync(otp);
    await _otpRepository.SaveChangesAsync();

    try
    {
        await _emailService.SendEmailAsync(
            user.Email!,
            "Password Reset OTP",
            $"Your OTP Code is: {otp.Code}"
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Email send failed: {ex.Message}");
    }

    return true;
}

    // ================= RESET PASSWORD =================
    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

        if (user is null)
            return Error.NotFound();

        var otp = await _otpRepository
            .GetValidOtpAsync(user.Id, resetPasswordDto.OtpCode, "PasswordReset");

        if (otp is null)
            return Error.Validation("OTP.Invalid", "Invalid or expired OTP");

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, resetPasswordDto.NewPassword);

        otp.IsUsed = true;
        await _otpRepository.SaveChangesAsync();

        return true;
    }

    // ================= JWT =================
    private async Task<string> CreateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>()
        {
            // 👇 ده السطر المهم اللي كان ناقص
            new Claim(ClaimTypes.NameIdentifier, user.Id),

            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var secretKey = _configuration["JwtOptions:SecretKey"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtOptions:Issuer"],
            audience: _configuration["JwtOptions:Audience"],
            expires: DateTime.UtcNow.AddHours(1),
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> CheckEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Error.NotFound();

        var token = await CreateTokenAsync(user);

        return new UserDto(user.Email!, user.DisplayName, token);
    }
}