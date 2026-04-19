using DNAAnalysis.Shared.IdentityDtos;
using DNAAnalysis.Shared.CommonResult;

namespace DNAAnalysis.Services.Abstraction;

public interface IAuthenticationService
{
    //login 
    //email, password==> token , Displayname, Email
    Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
     
     //Register
     //Email, pasword, username, dispalyname, phonenumber, > token, Displayname , email
    Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto);
    
    //CheckEmail (true/false)
    Task<bool> CheckEmailAsync(string Email);

    //GetUserByEmai
   Task<Result<UserDto>> GetUserByEmailAsync(string Email);

  Task<Result<bool>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

Task<Result<bool>> ResendConfirmationAsync(ResendConfirmationDto resendConfirmationDto);

Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);



}
