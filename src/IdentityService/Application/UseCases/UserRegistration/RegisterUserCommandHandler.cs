// مسیر: src/IdentityService/Application/UseCases/UserRegistration/RegisterUserCommandHandler.cs

using Application.Interfaces;
using Core.Entities;
using MediatR;

namespace Application.UseCases.UserRegistration;

/// <summary>
/// Handler برای پردازش RegisterUserCommand.
/// این کلاس منطق کسب‌وکار ثبت‌نام کاربر را پیاده‌سازی می‌کند.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // بررسی اینکه آیا کاربری با همین نام کاربری یا ایمیل وجود دارد یا خیر
        var existingUser = await _userRepository.FindByUsernameOrEmailAsync(
            request.Username,
            request.Email,
            cancellationToken);

        if (existingUser != null)
        {
            // اگر کاربری با همین نام کاربری یا ایمیل وجود داشت، خطا برمی‌گردانیم
            if (existingUser.Username == request.Username)
            {
                throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
            }
            if (existingUser.Email == request.Email)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
            }
        }

        // ایجاد کاربر جدید
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // افزودن کاربر به دیتابیس
        await _userRepository.AddAsync(newUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // برگرداندن شناسه کاربر جدید
        return newUser.Id;
    }
}

