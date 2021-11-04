using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    public class AddCityMessageHandler : IMessageHandler<AddOrUpdateCityMessage>
    {
        private readonly SearchEngineContext _context;

        public AddCityMessageHandler(SearchEngineContext context)
        {
            _context = context;
        }

        public async Task Handle(AddOrUpdateCityMessage message)
        {
            var cityInDb = await _context.Set<City>().Where(x => x.Code == message.Code).FirstOrDefaultAsync();

            if (cityInDb != null)
                return;
            
            var city = new City
            {
                Code = message.Code
            };

            _context.Add(city);

            await _context.SaveChangesAsync();            
        }
    }
}
