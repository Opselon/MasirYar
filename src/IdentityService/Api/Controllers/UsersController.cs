// مسیر: src/IdentityService/Api/Controllers/UsersController.cs

using Application.UseCases.UserRegistration;
using Application.UseCases.UserLogin;
using Api.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;

    public UsersController(IMediator mediator, IUserRepository userRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        // تبدیل DTO به Command
        var command = new RegisterUserCommand
        {
            Username = registerUserDto.Username,
            Email = registerUserDto.Email,
            Password = registerUserDto.Password
        };

        try
        {
            // ارسال Command به MediatR برای پردازش
            var userId = await _mediator.Send(command);

            // برگرداندن پاسخ موفقیت‌آمیز به همراه شناسه کاربر جدید
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { UserId = userId });
        }
        catch (InvalidOperationException ex)
        {
            // در صورت وجود خطا (مانند تکراری بودن نام کاربری یا ایمیل)
            return Conflict(new { Error = ex.Message });
        }
        catch (Exception)
        {
            // برای خطاهای غیرمنتظره
            return StatusCode(500, new { Error = "An error occurred while registering the user." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred while logging in." });
        }
    }

    /// <summary>
    /// دریافت اطلاعات چند کاربر به صورت Batch (برای حل مشکل N+1 Query)
    /// </summary>
    [HttpPost("batch")]
    [Authorize]
    public async Task<IActionResult> GetUsersByIds([FromBody] List<Guid> userIds)
    {
        try
        {
            var users = await _userRepository.FindByIdsAsync(userIds);
            var userDtos = users.Select(u => new
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            }).ToList();

            return Ok(userDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred while retrieving users." });
        }
    }

    [HttpGet("profile")]
    [Authorize] // این Endpoint اکنون محافظت‌شده است و نیاز به توکن دارد.
    public IActionResult GetProfile()
    {
        // از طریق Claims می‌توانیم به اطلاعات کاربر احراز هویت شده دسترسی پیدا کنیم.
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        
        if (userId is null) return Unauthorized();

        return Ok(new { UserId = userId, Username = username, Message = "This is a protected endpoint." });
    }

    // این یک Endpoint خالی است که فقط برای استفاده در CreatedAtAction تعریف شده است.
    // در آینده آن را پیاده‌سازی خواهیم کرد.
    [HttpGet("{id}")]
    public IActionResult GetUserById(Guid id)
    {
        return Ok($"User with id {id} - To be implemented.");
    }
}
