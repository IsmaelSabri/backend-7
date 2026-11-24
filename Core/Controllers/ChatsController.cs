using System.Security.Claims;
using Core.Collections;
using Core.Dto;
using Core.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Core.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public sealed class ChatsController(IChats chats, IUserBlocks blocks, IHubContext<ChatHub> hub, ILogger<ChatsController> logger) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetBlockStatus(Guid otherUserId, CancellationToken ct)
    {
        if (otherUserId == Guid.Empty || otherUserId == CurrentUserId) return BadRequest();
        var state = await blocks.GetStatus(CurrentUserId, otherUserId, ct);
        return Ok(new { state.BlockedByMe, state.BlockedMe });
    }

    [HttpPost]
    public async Task<IActionResult> BlockUser(BlockUserRequest request, CancellationToken ct)
    {
        try { await blocks.Block(CurrentUserId, request.UserId, ct); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (KeyNotFoundException) { return NotFound(); }
        await Notify(request.UserId, "BlockStatusChanged", new { UserId = CurrentUserId });
        await Notify(CurrentUserId, "BlockStatusChanged", new { request.UserId });
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> UnblockUser(BlockUserRequest request, CancellationToken ct)
    {
        if (request.UserId == Guid.Empty || request.UserId == CurrentUserId) return BadRequest();
        await blocks.Unblock(CurrentUserId, request.UserId, ct);
        await Notify(request.UserId, "BlockStatusChanged", new { UserId = CurrentUserId });
        await Notify(CurrentUserId, "BlockStatusChanged", new { request.UserId });
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations(CancellationToken ct) =>
        Ok(await chats.GetConversations(CurrentUserId, ct));

    [HttpGet]
    public async Task<IActionResult> GetMessages(Guid conversationId, CancellationToken ct)
    {
        var result = await chats.GetMessages(conversationId, CurrentUserId, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendChatRequest request, CancellationToken ct)
    {
        try
        {
            var message = await chats.SendMessage(CurrentUserId, request, ct);
            await Notify(message.ToUserId, "ChatReappeared", new { message.ConversationId });
            await Notify(message.ToUserId, "Messages", message);
            return Ok(message);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost]
    public async Task<IActionResult> ClearConversation(ConversationRequest request, CancellationToken ct)
    {
        return await chats.ClearConversation(request.ConversationId, CurrentUserId, ct) ? Ok() : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateChat(UpdateChatRequest request, CancellationToken ct)
    {
        try
        {
            var message = await chats.UpdateMessage(request.Id, CurrentUserId, request.State, ct);
            if (message != null)
                await Notify(message.UserId, message.State == "Read" ? "MessageRead" : "MessageReceived",
                    message.State == "Read" ? new[] { message.Id } : message.Id);
            return message == null ? NotFound() : Ok(message);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPut]
    public async Task<IActionResult> MarkConversationAsRead(ConversationRequest request, CancellationToken ct)
    {
        var updated = await chats.MarkRead(request.ConversationId, CurrentUserId, ct);
        if (updated == null) return NotFound();
        if (updated.Count > 0)
        {
            var conversation = (await chats.GetConversations(CurrentUserId, ct))
                .FirstOrDefault(c => c.ConversationId == request.ConversationId);
            if (conversation != null) await Notify(conversation.OtherUserId, "MessageRead", updated);
        }
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteChat(Guid id, CancellationToken ct)
    {
        var message = await chats.DeleteMessage(id, CurrentUserId, ct);
        if (message == null) return NotFound();
        await hub.Clients.User(message.ToUserId.ToString()).SendAsync("ChatDeleted", id, ct);
        return Ok();
    }

    private async Task Notify(Guid userId, string method, object data)
    {
        try
        {
            await hub.Clients.User(userId.ToString()).SendAsync(method, data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo enviar el evento {Method} al usuario {UserId}", method, userId);
        }
    }
}
