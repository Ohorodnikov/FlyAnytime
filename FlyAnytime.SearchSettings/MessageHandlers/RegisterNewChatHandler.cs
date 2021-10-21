using FlyAnytime.Messaging.Messages;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class RegisterNewChatHandler : IMessageHandler<RegisterNewChatMessage>
    {
        IRepository<User> _userRepo;
        IRepository<Chat> _chatRepo;
        public RegisterNewChatHandler(IRepository<User> userRepo, IRepository<Chat> chatRepo)
        {
            _userRepo = userRepo;
            _chatRepo = chatRepo;
        }

        public async Task Handle(RegisterNewChatMessage message)
        {
            var userResult = await _userRepo.GetBy(x => x.UserId, message.UserId.ToString());

            var savedUser = userResult.Entity;
            if (!userResult.Success)
            {
                var user = new User
                {
                    UserId = message.UserId.ToString(),
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    UserName = message.LastName
                };

                var saveResult = await _userRepo.TryCreate(user);

                if (!saveResult.Success)
                {
                    return;
                }

                savedUser = saveResult.Entity;
            }

            var chatResult = await _chatRepo.GetBy(x => x.ChatId, message.ChatId.ToString());

            if (chatResult.Success)
            {
                return;
            }

            var chat = new Chat
            {
                ChatId = message.ChatId.ToString(),
                ChatOwner = savedUser,
                IsGroup = message.IsGroup,
                Title = message.ChatName
            };

            await _chatRepo.TryCreate(chat);
        }
    }
}
