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
    public class ChangeChatCountryHandler : IMessageHandler<UpdateChatCountryMessage>
    {
        private readonly IRepository<Country> _countryRepo;
        private readonly IRepository<Chat> _chatRepo;
        IPublishEditChatSettingsHelper _publishHelper;
        public ChangeChatCountryHandler(IRepository<Country> countryRepo, IRepository<Chat> chatRepo, IPublishEditChatSettingsHelper publishHelper)
        {
            _countryRepo = countryRepo;
            _chatRepo = chatRepo;
            _publishHelper = publishHelper;
        }

        public async Task Handle(UpdateChatCountryMessage message)
        {
            var chatResult = await _chatRepo.GetOneBy(chat => chat.ChatId == message.ChatId);

            if (!chatResult.Success)
                return;

            var chat = chatResult.Entity;

            var flyFrom = await _countryRepo.GetOneBy(x => x.Code == message.CountryCode);

            if (chat.CountryFlyFromId == flyFrom.Entity.Id)
                return;

            chat.CountryFlyFrom = flyFrom.Entity;
            chat.CurrencyCode = chat.CountryFlyFrom.DefSearchCurrencyCode;

            chat.CityFlyFromId = MongoDB.Bson.ObjectId.Empty;

            var res = await _chatRepo.TryReplace(chat);
        }
    }
}
