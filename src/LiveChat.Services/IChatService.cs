using System.Collections.Generic;

using LiveChat.Infrastructure.DataTransferModels;

namespace LiveChat.Services
{
    public interface IChatService
    {
        IEnumerable<ChatResponse> GetUsersChats(int userId);

        CreateChatResponse Create(int userId, int creatorId);

        ChatDetailsResponse GetChat(int chatId, int userId);

        SendMessageResponse SendMessage(int chatId, SendMessageRequest sendMessageRequest, int userId);

        IList<int> UsersInChat(int chatId);
    }
}
