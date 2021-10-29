using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.Scheduler.EF;
using FlyAnytime.Scheduler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class DeleteSearchJobHandler : IMessageHandler<DeleteSearchJobMessage>
    {
        private readonly IJobHelper _jobHelper;
        private readonly ILogger _logger;
        public DeleteSearchJobHandler(IJobHelper jobHelper, ILogger<DeleteSearchJobHandler> logger)
        {
            _jobHelper = jobHelper;
            _logger = logger;
        }

        public async Task Handle(DeleteSearchJobMessage message)
        {
            await _jobHelper.DeleteJobsForChatSettings(message.ChatId, message.SettingsId);
        }
    }
}
