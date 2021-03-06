﻿using System;
using System.Linq;
using System.Collections.Generic;

using LiveChat.Data;
using LiveChat.Infrastructure.DataTransferModels;
using LiveChat.Data.Models;

namespace LiveChat.Services
{
    public class ChatService : IChatService
    {
        private LiveChatDbContext _context;

        public ChatService(LiveChatDbContext context)
        {
            this._context = context;
        }

        public CreateChatResponse Create(int userId, int creatorId)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) throw new InvalidOperationException();

                var existingChat = (from c in this._context.Chats
                                    join uc in this._context.UserChats on c.Id equals uc.ChatId
                                    where (c.UserChats.Count == 2) && (uc.UserId == creatorId || uc.UserId == userId)
                                    group uc by uc.ChatId into g
                                    where g.Count() == 2
                                    select new
                                    {
                                        Id = g.Key
                                    }).FirstOrDefault();

                if (existingChat != null)
                {
                    return new CreateChatResponse
                    {
                        ChatId = existingChat.Id,
                        Message = "This chat already exist!"
                    };
                }

                Chat chat = new Chat();
                this._context.Chats.Add(chat);
                this._context.SaveChanges();

                this._context.UserChats.Add(new UserChat { ChatId = chat.Id, UserId = user.Id });
                this._context.UserChats.Add(new UserChat { ChatId = chat.Id, UserId = creatorId });
                this._context.SaveChanges();

                return new CreateChatResponse
                {
                    ChatId = chat.Id
                };
            }
            catch (Exception ex)
            {
                return new CreateChatResponse
                {
                    ChatId = -1,
                    Message = ex.Message
                };
            }
        }

        public ChatDetailsResponse GetChat(int chatId, int userId)
        {
            // TODO: Add paging
            // TODO: Add validation for userId

            var chat = this._context.Chats
                .Where(c => c.Id == chatId)
                .Select(c => new ChatDetailsResponse
                {
                    Id = c.Id,
                    Users = c.UserChats
                        .Select(uc => new UserInChatModel
                        {
                            Id = uc.User.Id,
                            FirstName = uc.User.FirstName,
                            LastName = uc.User.LastName,
                            Email = uc.User.Email
                        }),
                    Messages = c.Messages
                        .Select(m => new MessageModel
                        {
                            Id = m.Id,
                            DateTime = m.Date,
                            AuthorId = m.AuthorId.Value,
                            AuthorFirstName = m.Author.FirstName,
                            AuthoeLastName = m.Author.LastName,
                            ProfilePictureUrl = "images/" + m.Author.ProfilePicturePath,
                            Text = m.Text
                        })
                        .OrderBy(m => m.DateTime)
                })
                .FirstOrDefault();

            if (chat == null)
                return null;

            return chat;
        }

        public IEnumerable<ChatResponse> GetUsersChats(int userId)
        {
            var myChatIds = (from c in this._context.Chats
                             join uc in this._context.UserChats on c.Id equals uc.ChatId
                             join u in this._context.Users on uc.UserId equals u.Id
                             where u.Id == userId
                             select c.Id).ToList();

            if (myChatIds.Count < 1)
                return null;

            var myChats = this._context.Chats
                .Where(c => myChatIds.Contains(c.Id))
                .Select(c => new
                {
                    Id = c.Id,
                    Users = c.UserChats.Select(uc => new { uc.User.FirstName, uc.User.LastName }),
                    LastMessage = c.Messages.OrderByDescending(m => m.Date).Select(m => new { m.Author.FirstName, m.Author.LastName, m.Text, m.Date }).Take(1)
                })
                .ToList();

            var result = new List<ChatResponse>();

            foreach (var chat in myChats)
            {
                var chatResponse = new ChatResponse();
                var lastMessage = chat.LastMessage.FirstOrDefault();

                if (lastMessage != null)
                {
                    chatResponse.Id = chat.Id;
                    chatResponse.Users = chat.Users.Select(u => string.Format("{0} {1}", u.FirstName, u.LastName));
                    chatResponse.LastMessageText = lastMessage.Text;
                    chatResponse.LastMessageAuthor = string.Format("{0} {1}", lastMessage.FirstName, lastMessage.LastName);
                    chatResponse.LastMessageDate = lastMessage.Date;
                }
                else
                {
                    chatResponse.Id = chat.Id;
                    chatResponse.Users = chat.Users.Select(u => string.Format("{0} {1}", u.FirstName, u.LastName));
                    chatResponse.LastMessageText = "";
                    chatResponse.LastMessageAuthor = "";
                    chatResponse.LastMessageDate = DateTime.UtcNow;
                }

                result.Add(chatResponse);
            }

            return result.OrderByDescending(r => r.LastMessageDate).ToList();
        }

        public SendMessageResponse SendMessage(int chatId, SendMessageRequest sendMessageRequest, int userId)
        {
            try
            {
                var usersInChat = this._context.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.UserId)
                .ToArray();

                if (!usersInChat.Contains(userId))
                    throw new InvalidOperationException("Current user is not a member of this chat!");

                var message = new Message
                {
                    AuthorId = userId,
                    ChatId = chatId,
                    Date = sendMessageRequest.UtcDate,
                    Text = sendMessageRequest.Text
                };

                this._context.Messages.Add(message);

                this._context.SaveChanges();

                return new SendMessageResponse
                {
                    MessageId = message.Id
                };
            }
            catch (Exception ex)
            {
                return new SendMessageResponse
                {
                    MessageId = -1,
                    Message = ex.Message
                };
            }

        }

        public IList<int> UsersInChat(int chatId)
        {
            return this._context.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.UserId)
                .ToList();
        }
    }
}
