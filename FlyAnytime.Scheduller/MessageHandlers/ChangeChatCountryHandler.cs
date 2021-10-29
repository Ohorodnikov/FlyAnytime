using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.ChatSettings;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class ChangeChatCountryHandler : IMessageHandler<UpdateChatCountryMessage>
    {
        private readonly SchedulerDbContext _dbContext;

        public ChangeChatCountryHandler(SchedulerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateChatCountryMessage message)
        {
            await SetNewCityFromAsNull<FixedDateSearchJobData>(message.ChatId);
            await SetNewCityFromAsNull<DynamicDateSearchJobData>(message.ChatId);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SetNewCityFromAsNull<TJobData>(long chatId)
            where TJobData : BaseSearchJobData
        {
            var data = await _dbContext.Set<TJobData>().Where(x => x.ChatId == chatId).ToListAsync();

            foreach (var d in data)
                d.CityFlyFrom = null;
        }
    }
}
