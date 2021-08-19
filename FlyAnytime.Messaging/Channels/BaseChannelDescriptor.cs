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

        string Data2String();
        TChannelData Message2Data(string message);
    }

    public interface IChannelWithResultDescriptor<TChannelData, TChannelResultData> : IChannelDescriptor<TChannelData>
        where TChannelResultData : IChannelData
        where TChannelData : IChannelData
    {
        TChannelResultData ResultString2Data(string resultString);
        string ResultData2String(TChannelResultData data);
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

        public virtual string Data2String()
        {
            return JsonConvert.SerializeObject(Data);
        }

        public virtual TChannelData Message2Data(string message)
        {
            return JsonConvert.DeserializeObject<TChannelData>(message);
        }
    }

    public abstract class BaseChannelWithResultDescriptor<TChannelData, TChannelResultData> : BaseChannelDescriptor<TChannelData>, IChannelWithResultDescriptor<TChannelData, TChannelResultData>
        where TChannelResultData : IChannelData
        where TChannelData : IChannelData
    {
        public BaseChannelWithResultDescriptor(string channelName) : base(channelName) { }

        public string ResultData2String(TChannelResultData data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public TChannelResultData ResultString2Data(string resultString)
        {
            return JsonConvert.DeserializeObject<TChannelResultData>(resultString);
        }
    }
}
