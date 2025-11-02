// مسیر: src/IdentityService/Api/Controllers/JournalsController.cs

using System.Security.Claims;
using Application.UseCases.Journals.CreateJournal;
using Application.UseCases.Journals.GetJournals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // این کنترلر و تمام Endpoint های آن نیاز به احراز هویت دارند
public class JournalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JournalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ابزار کمکی برای گرفتن شناسه کاربر از توکن JWT
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            // این اتفاق نباید در یک Endpoint محافظت‌شده رخ دهد، اما برای اطمینان
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> CreateJournal([FromBody] CreateJournalCommand command)
    {
        try
        {
            // اطمینان از اینکه کاربر فقط برای خودش ژورنال می‌سازد
            command.UserId = GetCurrentUserId();
            
            var journalId = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetJournalById), new { id = journalId }, new { JournalId = journalId });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred while creating the journal." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyJournals()
    {
        try
        {
            var query = new GetJournalsQuery { UserId = GetCurrentUserId() };
            var journals = await _mediator.Send(query);
            return Ok(journals);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred while retrieving journals." });
        }
    }

    // یک Endpoint خالی برای استفاده در CreatedAtAction
    [HttpGet("{id}")]
    public IActionResult GetJournalById(Guid id)
    {
        return Ok($"Journal with id {id} - To be implemented.");
    }
}

