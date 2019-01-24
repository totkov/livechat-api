using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using LiveChat.Services;
using LiveChat.Infrastructure.DataTransferModels;
using LiveChat.API.Hubs;

namespace LiveChat.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/chats/[action]")]
    public class ChatsController : BaseController
    {
        private IChatService _chatService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChatsController(
            IChatService chatService,
            IHubContext<NotificationHub> hubContext)
        {
            this._chatService = chatService;
            this._hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetMyChats()
        {
            var myChats = this._chatService.GetUsersChats(this.LoggedUserId);

            return this.Ok(myChats);
        }

        [HttpGet]
        public IActionResult GetChat(int id)
        {
            var chat = this._chatService.GetChat(id, this.LoggedUserId);

            if (chat == null)
                return this.BadRequest(new ChatDetailsResponse { Id = -1 });
            else
                return this.Ok(chat);
        }

        [HttpPost]
        public IActionResult CreateChat([FromBody] CreateChatRequest model)
        {
            if (this.ModelState.IsValid)
            {
                var result = this._chatService.Create(model, this.LoggedUserId);

                if (result.ChatId != -1)
                    return this.Ok(result);
                else
                    return this.BadRequest(result);
            }

            return this.BadRequest(new CreateChatResponse { ChatId = -1, Message = this.ModelState.Values.ToString() });
        }

        [HttpPost]
        public IActionResult SendMessage([FromQuery] int chatId, [FromBody] SendMessageRequest model)
        {
            if (this.ModelState.IsValid)
            {
                var result = this._chatService.SendMessage(chatId, model, this.LoggedUserId);
                var usersInChat = this._chatService.UsersInChat(chatId);
                var connectionIds = ConnectionHelper.GetAllConnections()
                    .Where(c => usersInChat.Contains(c.Key))
                    .Select(c => c.Value)
                    .ToList();

                foreach (var id in connectionIds)
                {
                    this._hubContext.Clients.Client(id).SendAsync("updateChats");
                }

                //this._hubContext.Clients.All.SendAsync("updateChats");

                if (result.MessageId != -1)
                    return this.Ok(result);
                else
                    return this.BadRequest(result);
            }

            return this.BadRequest(new SendMessageResponse { MessageId = -1, Message = this.ModelState.Values.ToString() });
        }
    }
}
