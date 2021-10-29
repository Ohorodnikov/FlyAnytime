using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.Scheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchEngine
{
    public class SearchFrame
    {
        public SearchFrame(long start,
                           long end,
                           Dictionary<Days, HashSet<byte>> allowedDateTimeSlotsTo,
                           Dictionary<Days, HashSet<byte>> allowedDateTimeSlotsBack)
        {
            Start = start;
            End = end;
            AllowedDateTimeSlotsTo = allowedDateTimeSlotsTo;
            AllowedDateTimeSlotsBack = allowedDateTimeSlotsBack;
        }

        public long Start { get; }
        public long End { get; }
        public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsTo { get; }
        public Dictionary<Days, HashSet<byte>> AllowedDateTimeSlotsBack { get; }

    }

    public class MakeSearchMessage : BaseMessage
    {
        public MakeSearchMessage(long chatId,
                                 string cityFlyFrom,
                                 IEnumerable<string> airportsFlyTo,
                                 PriceSettings priceSettings,
                                 TripDuration tripDuration,
                                 SearchFrame searchFrame)
        {
            ChatId = chatId;
            CityFlyFrom = cityFlyFrom;
            AirportsFlyTo = airportsFlyTo;
            PriceSettings = priceSettings;
            TripDuration = tripDuration;
            SearchFrame = searchFrame;
        }

        public long ChatId { get; }

        public string CityFlyFrom { get; }
        public IEnumerable<string> AirportsFlyTo { get; }

        public PriceSettings PriceSettings { get;}
        public TripDuration TripDuration { get; }

        public SearchFrame SearchFrame { get; }        
    }
}
