using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class ErrorDuringSearchHandler : IMessageHandler<ErrorDuringSearchMessage>
    {
        private readonly ITelegramBotClient _tgClient;

        public ErrorDuringSearchHandler(ITelegramBotClient tgClient)
        {
            _tgClient = tgClient;
        }

        public async Task Handle(ErrorDuringSearchMessage message)
        {
            await _tgClient.SendTextMessageAsync(message.ChatId, message.Message);
        }
    }
}
