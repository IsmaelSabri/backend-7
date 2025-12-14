using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Enums;
using Users.Hubs;
using Users.Models;
using Sieve.Services;
using Sieve.Models;

namespace Users.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatsController(UserDb context, IHubContext<ChatHub> hubContext, SieveProcessor sieveProcessor) : ControllerBase
    {

        private readonly SieveProcessor _sieveProcessor = sieveProcessor;

        [HttpGet]
        public async Task<IActionResult> GetChats(string userId, string toUserId, string ViviendaId, CancellationToken cancellationToken)
        {
            List<Chat> chats =
                await context
                .Chats
                .Where(p =>
                ((p.UserId == userId && p.ToUserId == toUserId) || (p.ToUserId == userId && p.UserId == toUserId)) && p.ViviendaId == ViviendaId)
                .OrderBy(p => p.Date)
                //.Where(v => v.ViviendaId==ViviendaId)
                .ToListAsync(cancellationToken);

            return Ok(chats);
        }

        [HttpGet]
        public IActionResult GetChatsFiltered([FromQuery] SieveModel sieveModel)
        {
            var result = _sieveProcessor.Apply(sieveModel, context.Chats.AsQueryable().AsNoTracking().OrderBy(p => p.Date));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChat(Chat chat, CancellationToken cancellationToken)
        {
            var existingChat = await context.Chats.FindAsync(new object[] { chat.Id }, cancellationToken: cancellationToken);

            if (existingChat == null)
                return NotFound("Chat no encontrado");

            //existingChat.Message = chat.Message;
            existingChat.State = chat.State;
            //existingChat.Date = DateTime.Now;

            context.Chats.Update(existingChat);
            await context.SaveChangesAsync(cancellationToken);
            return Ok(existingChat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto request, CancellationToken cancellationToken)
        {
            Chat chat = new()
            {
                UserId = request.UserId,
                ToUserId = request.ToUserId,
                Message = request.Message,
                Date = DateTime.Now,
                State = request.State,
                ViviendaId = request.ViviendaId,
            };

            await context.AddAsync(chat, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Busca si el usuario está conectado
            var connectionId = ChatHub.Users.FirstOrDefault(p => p.Value == chat.ToUserId).Key;

            // Solo envía si está conectado
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("Messages", chat, cancellationToken: cancellationToken);
            }

            return Ok(chat);
        }
    }

}