using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Messaging.Channels
{
    public interface IChannelDescriptor<TChannelData>
        where TChannelData : IChannelData
    {
        string ChannelName { get; }

        TChannelData Data { get; set; }

        string GetStringData();
    }

    public abstract class BaseChannelDescriptor<TChannelData> : IChannelDescriptor<TChannelData>
        where TChannelData : IChannelData
    {
        public BaseChannelDescriptor(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; }
        public TChannelData Data { get; set; }

        public string GetStringData()
        {
            return JsonConvert.SerializeObject(Data);
        }
    }
}
