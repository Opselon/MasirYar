// مسیر: src/IdentityService/Application/UseCases/UserLogin/LoginUserCommandHandler.cs

using Application.Interfaces;
using MediatR;

namespace Application.UseCases.UserLogin;

/// <summary>
/// Handler برای پردازش LoginUserCommand.
/// این کلاس منطق کسب‌وکار ورود کاربر را پیاده‌سازی می‌کند.
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // یافتن کاربر بر اساس نام کاربری
        var user = await _userRepository.FindByUsernameAsync(request.Username, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // بررسی صحت رمز عبور
        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // تولید JWT Token
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Username);

        // برگرداندن نتیجه
        return new LoginUserResult
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username
        };
    }
}

