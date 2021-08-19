using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Channels
{
    public class GetLoginLinkChannelData : BaseChannelData
    {
        public long UserId { get; set; }
    }

    public class LoginLinkResult : BaseChannelData
    {
        public string Url { get; set; }
    }

    public class GetLoginLinkChannel : BaseChannelWithResultDescriptor<GetLoginLinkChannelData, LoginLinkResult>
    {
        public GetLoginLinkChannel() : base("getLoginUrl") { }
    }
}
