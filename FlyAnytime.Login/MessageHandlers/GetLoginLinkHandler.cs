using FlyAnytime.Login.EF;
using FlyAnytime.Login.Helpers;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.MessageHandlers
{
    public class GetLoginLinkHandler : IMessageHandler<GetLoginLinkRequestMessage, GetLoginLinkResponseMessage>
    {
        private IOclHelper _oclHelper;
        public GetLoginLinkHandler(IOclHelper oclHelper)
        {
            _oclHelper = oclHelper;
        }

        public async Task<GetLoginLinkResponseMessage> Handle(GetLoginLinkRequestMessage message)
        {
            var ocl = await _oclHelper.Create(message.UserId);

            return new GetLoginLinkResponseMessage(ocl?.LoginUrl);
        }
    }
}
