using FlyAnytime.Messaging.Messages;
using FlyAnytime.Scheduler.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class DeleteAllUsersDataHandler : IMessageHandler<DeleteAllUsersDataMessage>
    {
        private readonly SchedulerDbContext _dbContext;

        public DeleteAllUsersDataHandler(SchedulerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteAllUsersDataMessage message)
        {
            await _dbContext.ReCreateDb();
        }
    }
}
