using FlyAnytime.Messaging.Messages;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class AddNewLanguageHandler : IMessageHandler<AddNewLanguageMessage>
    {
        private readonly TelegramContext _dbContext;

        public AddNewLanguageHandler(TelegramContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(AddNewLanguageMessage message)
        {
            if (_dbContext.Set<Language>().Any(x => x.Code == message.Code))
                return;
            
            var lang = new Language
            {
                Code = message.Code,
                Name = message.Name,
                Culture = message.Culture
            };

            _dbContext.Add(lang);

            await _dbContext.SaveChangesAsync();
        }
    }
}
