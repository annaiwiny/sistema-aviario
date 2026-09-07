using FarmSystemProject.DTOs.Notifications;
using FarmSystemProject.Interfaces.INotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/notifications")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetAll()
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAll(userId);
        return Ok(response);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount()
    {
        var userId = GetUserIdFromToken();
        var count = await _service.GetUnreadCount(userId);
        return Ok(new UnreadCountResponse { Count = count });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserIdFromToken();
        await _service.MarkAsRead(id, userId);
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserIdFromToken();
        await _service.MarkAllAsRead(userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromToken();
        await _service.Delete(id, userId);
        return NoContent();
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}
