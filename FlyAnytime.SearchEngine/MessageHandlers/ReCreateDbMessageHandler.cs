using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    public class ReCreateDbMessageHandler : IMessageHandler<ReCreateDbMessage>
    {
        private readonly SearchEngineContext _context;

        public ReCreateDbMessageHandler(SearchEngineContext context)
        {
            _context = context;
        }

        public async Task Handle(ReCreateDbMessage message)
        {
            await _context.Set<Airport>().RemoveAll();
            await _context.Set<City>().RemoveAll();
            //throw new NotImplementedException();
        }
    }
}
