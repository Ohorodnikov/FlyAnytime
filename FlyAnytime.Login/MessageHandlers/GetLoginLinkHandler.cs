using FlyAnytime.Core;
using FlyAnytime.Login.Helpers;
using FlyAnytime.Messaging.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FlyAnytime.Login.MessageHandlers
{
    public class GetLoginLinkHandler : IMessageHandler<GetLoginLinkRequestMessage, GetLoginLinkResponseMessage>
    {
        private IOclHelper _oclHelper;
        private string GatewayUrl;
        public GetLoginLinkHandler(IOclHelper oclHelper, ICommonSettings configuration)
        {
            _oclHelper = oclHelper;
            GatewayUrl = configuration.ApiGatewayUrl;
        }

        public async Task<GetLoginLinkResponseMessage> Handle(GetLoginLinkRequestMessage message)
        {
            var ocl = await _oclHelper.Create(message.UserId);

            var link = $"{GatewayUrl}/auth/jwtocl/{ocl?.LoginUrl}";

            return new GetLoginLinkResponseMessage(link);
        }
    }
}
