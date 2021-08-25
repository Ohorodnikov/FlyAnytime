using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages
{
    public class GetLoginLinkRequestMessage : BaseMessage
    {
        public GetLoginLinkRequestMessage(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; set; }
    }
    public class GetLoginLinkResponseMessage : BaseResponseMessage<GetLoginLinkRequestMessage>
    {
        private GetLoginLinkResponseMessage() { }

        public GetLoginLinkResponseMessage(string url)
        {
            LoginUrl = url;
            if (LoginUrl is null)
                ErrorMessage = "User was not found";
        }

        public string LoginUrl { get; set; }
    }

}
