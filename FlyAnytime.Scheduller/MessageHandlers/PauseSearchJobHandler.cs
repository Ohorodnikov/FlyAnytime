using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.Scheduler;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Scheduler.MessageHandlers
{
    public class PauseSearchJobHandler : IMessageHandler<PauseSearchJobMessage>
    {
        private readonly IJobHelper _jobHelper;
        private readonly ILogger _logger;
        public PauseSearchJobHandler(IJobHelper jobHelper, ILogger<PauseSearchJobHandler> logger)
        {
            _jobHelper = jobHelper;
            _logger = logger;
        }

        public async Task Handle(PauseSearchJobMessage message)
        {
            var group = _jobHelper.CreateSearchJobGroupName(message.ChatId, message.SettingsId);
            await _jobHelper.PauseJobsByGroup(group);
        }
    }
}
