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
    public class ChangeChatCityHandler : IMessageHandler<UpdateChatCityMessage>
    {
        private readonly SchedulerDbContext _dbContext;

        public ChangeChatCityHandler(SchedulerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateChatCityMessage message)
        {
            await SetNewCityFrom<FixedDateSearchJobData>(message.ChatId, message.CityCode);
            await SetNewCityFrom<DynamicDateSearchJobData>(message.ChatId, message.CityCode);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SetNewCityFrom<TJobData>(long chatId, string cityFromCode)
            where TJobData : BaseSearchJobData
        {
            var data = await _dbContext.Set<TJobData>().Where(x => x.ChatId == chatId).ToListAsync();

            foreach (var d in data)
                d.CityFlyFrom = cityFromCode;
        }
    }
}
