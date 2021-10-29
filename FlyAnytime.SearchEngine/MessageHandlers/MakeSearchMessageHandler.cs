using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchEngine;
using FlyAnytime.SearchEngine.Exceptions;
using SearchEngine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.MessageHandlers
{
    public class MakeSearchMessageHandler : IMessageHandler<MakeSearchMessage>
    {
        private readonly ISearchEngine _searchEngine;
        private readonly IMessageBus _messageBus;

        public MakeSearchMessageHandler(ISearchEngine searchEngine, IMessageBus messageBus)
        {
            _searchEngine = searchEngine;
            _messageBus = messageBus;
        }

        public async Task Handle(MakeSearchMessage message)
        {
            try
            {
                var results = await _searchEngine.Search(message);

                var msgRes = results
                    .Select(r => new OneSearchResult(r.CityFrom, r.CityTo, r.DateTimeFrom, r.DateTimeBack, r.Price, r.DiscountPercent, r.ResultUrl))
                    .ToList();

                var resultMsg = new SearchResultMessage(message.ChatId, msgRes);

                _messageBus.Publish(resultMsg);
            }
            catch (DataValidationException ex)
            {
                var msg = new ErrorDuringSearchMessage(ex.Message, message.ChatId);
                _messageBus.Publish(msg);
            }
        }
    }
}
