using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    class AppInitMessageHandler : IMessageHandler<AppInitMessage>
    {
        public async Task Handle(AppInitMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}
