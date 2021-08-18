using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Channels
{
    public class CreateUserChannelData : BaseChannelData
    {
    }

    public class CreateUserChannel : BaseChannelDescriptor<CreateUserChannelData>
    {
        public CreateUserChannel() : base("CREATE_USER") { }
    }
}
