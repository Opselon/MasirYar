// مسیر: src/IdentityService/Application/UseCases/UserRegistration/RegisterUserCommand.cs

using MediatR;

namespace Application.UseCases.UserRegistration;

/// <summary>
/// Command برای ثبت‌نام کاربر جدید.
/// این Command با استفاده از MediatR به Handler مربوطه ارسال می‌شود.
/// </summary>
public class RegisterUserCommand : IRequest<Guid>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

