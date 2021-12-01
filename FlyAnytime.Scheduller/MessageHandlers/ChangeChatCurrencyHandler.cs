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
    public class ChangeChatCurrencyHandler : IMessageHandler<ChangeChatCurrencyMessage>
    {
        private readonly SchedulerDbContext _dbContext;

        public ChangeChatCurrencyHandler(SchedulerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(ChangeChatCurrencyMessage message)
        {
            await SetNewCurrCode<FixedDateSearchJobData>(message.ChatId, message.CurrencyCode);
            await SetNewCurrCode<DynamicDateSearchJobData>(message.ChatId, message.CurrencyCode);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SetNewCurrCode<TJobData>(long chatId, string currCode)
            where TJobData : BaseSearchJobData
        {
            var data = await _dbContext.Set<TJobData>().Where(x => x.ChatId == chatId).ToListAsync();

            foreach (var d in data)
                d.Currency = currCode;
        }
    }
}
