using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class ChangeChatCurrencyHandler : IMessageHandler<ChangeChatCurrencyMessage>
    {
        private readonly IRepository<Chat> _chatRepo;

        public ChangeChatCurrencyHandler(IRepository<Chat> chatRepo)
        {
            _chatRepo = chatRepo;
        }

        public async Task Handle(ChangeChatCurrencyMessage message)
        {
            var chatResult = await _chatRepo.GetOneBy(chat => chat.ChatId == message.ChatId);

            if (!chatResult.Success)
                return;

            var chat = chatResult.Entity;

            if (chat.CurrencyCode == message.CurrencyCode)
                return;

            chat.CurrencyCode = message.CurrencyCode;

            var res = await _chatRepo.TryReplace(chat);
        }
    }
}
