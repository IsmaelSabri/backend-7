using Core.Data;
using Core.Dto;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpPost("events")]
    public async Task<IActionResult> TrackEvent([FromBody] TrackEvent trackEvent, CancellationToken cancellationToken)
    {
        if (trackEvent is null)
        {
            return BadRequest();
        }

        try
        {
            await analyticsService.TrackAsync(trackEvent, cancellationToken);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("identity/resolve")]
    public async Task<IActionResult> ResolveIdentity([FromBody] AnalyticsIdentityResolveDto request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        try
        {
            await analyticsService.ResolveIdentityAsync(request.AnonymousId, request.SessionId, request.UserId, cancellationToken);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(await analyticsService.GetOverviewAsync(start, end, cancellationToken));
    }

    [HttpGet("emails")]
    public async Task<IActionResult> GetEmails([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(await analyticsService.GetEmailMetricsAsync(start, end, cancellationToken));
    }

    [HttpGet("contacts")]
    public async Task<IActionResult> GetContacts([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(await analyticsService.GetContactMetricsAsync(start, end, cancellationToken));
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetChats([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(await analyticsService.GetChatMetricsAsync(start, end, cancellationToken));
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(new { byDay = await analyticsService.GetActivityAsync(start, end, cancellationToken) });
    }

    [HttpGet("properties")]
    public async Task<IActionResult> GetProperties([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var start = (from ?? DateTime.UtcNow.AddDays(-7)).Date;
        var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

        return Ok(await analyticsService.GetPropertiesAsync(start, end, cancellationToken));
    }
}
