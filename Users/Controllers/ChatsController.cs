using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Users.Collections;
using Users.Enums;
using Users.Hubs;
using Users.Models;
using Sieve.Services;
using Sieve.Models;

namespace Users.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatsController(IChats chatsDb, IHubContext<ChatHub> hubContext, SieveProcessor sieveProcessor) : ControllerBase
    {

        private readonly SieveProcessor _sieveProcessor = sieveProcessor;

        [HttpGet]
        public async Task<IActionResult> GetChats(string userId, string toUserId, string ViviendaId, CancellationToken cancellationToken)
        {
            var chats = await chatsDb.GetChats(userId, toUserId, ViviendaId, cancellationToken);
            return Ok(chats);
        }

        [HttpGet]
        public IActionResult GetChatsFiltered([FromQuery] SieveModel sieveModel)
        {
            var result = _sieveProcessor.Apply(sieveModel, chatsDb.GetChatsQueryable().AsNoTracking().OrderBy(p => p.Date));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChat(Chat chat, CancellationToken cancellationToken)
        {
            if (chat == null)
                return BadRequest("Invalid chat payload");

            try
            {
                var updatedChat = await chatsDb.UpdateChat(chat, cancellationToken);
                return Ok(updatedChat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Chat no encontrado");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.ToUserId))
            {
                return BadRequest("Invalid message payload");
            }

            Chat chat = new()
            {
                UserId = request.UserId,
                ToUserId = request.ToUserId,
                Message = request.Message,
                Date = DateTime.Now,
                State = request.State,
                ViviendaId = request.ViviendaId,
            };

            var createdChat = await chatsDb.CreateChat(chat, cancellationToken);

            // Busca si el usuario está conectado
            var connectionId = ChatHub.Users.FirstOrDefault(p => p.Value == createdChat.ToUserId).Key;

            // Solo envía si está conectado
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("Messages", createdChat, cancellationToken: cancellationToken);
            }

            return Ok(createdChat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Invalid id");

            var existing = await chatsDb.GetChatById(id, cancellationToken);
            if (existing == null) return NotFound("Chat not found");

            await chatsDb.DeleteChat(existing, cancellationToken);

            // Notificar a los participantes conectados
            var participantIds = new[] { existing.UserId, existing.ToUserId }?.Where(pid => !string.IsNullOrWhiteSpace(pid)).Distinct();
            if (participantIds != null)
            {
                foreach (var pid in participantIds)
                {
                    var conn = ChatHub.Users.FirstOrDefault(p => p.Value == pid).Key;
                    if (!string.IsNullOrWhiteSpace(conn))
                    {
                        await hubContext.Clients.Client(conn).SendAsync("ChatDeleted", id, cancellationToken: cancellationToken);
                    }
                }
            }

            return Ok(new { deleted = id });
        }


    }

}