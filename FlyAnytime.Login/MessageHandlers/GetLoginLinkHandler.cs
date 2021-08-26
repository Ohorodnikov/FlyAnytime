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
        public GetLoginLinkHandler(IOclHelper oclHelper, IConfiguration configuration)
        {
            _oclHelper = oclHelper;
            GatewayUrl = configuration.GetSection("GatewayUrl").Value;
        }

        public async Task<GetLoginLinkResponseMessage> Handle(GetLoginLinkRequestMessage message)
        {
            var ocl = await _oclHelper.Create(message.UserId);

            var link = $"{GatewayUrl}/auth/jwtocl/{ocl?.LoginUrl}";

            return new GetLoginLinkResponseMessage(link);
        }
    }
}
