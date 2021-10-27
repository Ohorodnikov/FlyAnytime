using FlyAnytime.Core;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class ReCreateDbMessageHandler : IMessageHandler<ReCreateDbMessage>
    {
        private readonly IDbContextBase _dbContext;
        public ReCreateDbMessageHandler(IDbContextBase dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(ReCreateDbMessage message)
        {
            await _dbContext.ReCreateDb();
        }
    }
}
