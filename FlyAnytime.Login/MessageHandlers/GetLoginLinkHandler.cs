using FlyAnytime.Login.Helpers;
using FlyAnytime.Messaging.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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

            var link = $"https://localhost:44320/jwtocl/{ocl?.LoginUrl}";

            return new GetLoginLinkResponseMessage(link);
        }
    }
}
