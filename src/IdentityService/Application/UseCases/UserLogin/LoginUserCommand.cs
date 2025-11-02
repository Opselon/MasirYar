// مسیر: src/IdentityService/Application/UseCases/UserLogin/LoginUserCommand.cs

using MediatR;

namespace Application.UseCases.UserLogin;

/// <summary>
/// Command برای ورود کاربر به سیستم.
/// این Command با استفاده از MediatR به Handler مربوطه ارسال می‌شود.
/// </summary>
public class LoginUserCommand : IRequest<LoginUserResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه عملیات ورود کاربر
/// </summary>
public class LoginUserResult
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

