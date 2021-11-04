using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.SearchSettings;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    public class AddAirportMessageHandler : IMessageHandler<AddOrUpdateAirportMessage>
    {
        private readonly SearchEngineContext _context;

        public AddAirportMessageHandler(SearchEngineContext context)
        {
            _context = context;
        }

        public async Task Handle(AddOrUpdateAirportMessage message)
        {
            var city = await _context.Set<City>().Where(x => x.Code == message.CityCode).FirstOrDefaultAsync();

            if (city == null)
                return;

            var airport = await _context.Set<Airport>().Where(x => x.Code == message.Code).FirstOrDefaultAsync();

            if (airport != null)
                return;
            
            var airp = new Airport
            {
                Code = message.Code,
                City = city
            };

            _context.Add(airp);

            await _context.SaveChangesAsync();            
        }
    }
}
