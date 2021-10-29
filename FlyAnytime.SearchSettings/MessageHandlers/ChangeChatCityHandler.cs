using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.SearchSettings.Helpers;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class ChangeChatCityHandler : IMessageHandler<UpdateChatCityMessage>
    {
        private readonly IRepository<City> _cityRepo;
        private readonly IRepository<Chat> _chatRepo;
        public ChangeChatCityHandler(IRepository<City> cityRepo, IRepository<Chat> chatRepo)
        {
            _cityRepo = cityRepo;
            _chatRepo = chatRepo;
        }

        public async Task Handle(UpdateChatCityMessage message)
        {
            var chatResult = await _chatRepo.GetOneBy(chat => chat.ChatId == message.ChatId);

            if (!chatResult.Success)
                return;

            var chat = chatResult.Entity;

            var flyFrom = await _cityRepo.GetOneBy(x => x.Code == message.CityCode);

            if (chat.CityFlyFromId == flyFrom.Entity.Id)
                return;

            chat.CityFlyFrom = flyFrom.Entity;

            await _chatRepo.TryReplace(chat);           
        }
    }
}
