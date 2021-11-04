using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    public class ReCreateDbMessageHandler : IMessageHandler<ReCreateDbMessage>
    {
        public async Task Handle(ReCreateDbMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}
