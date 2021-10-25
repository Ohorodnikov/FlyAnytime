using FlyAnytime.Core.Enums;
using FlyAnytime.Messaging.Messages.Scheduler;
using FlyAnytime.SearchSettings.Models.Location;
using FlyAnytime.SearchSettings.Models.SearchSettings;
using FlyAnytime.SearchSettings.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Helpers
{
    public interface IChatSettingsHelper
    {
        Task<IEnumerable<Airport>> GetDestinationAirports(ChatSearchSettings searchSettings);
        Dictionary<Days, HashSet<byte>> GetDayTimeSlots(IEnumerable<FlyDaySettings> flyDaySettings);
        ScheduleSettings CreateScheduleSettings(Schedule schedule);
    }

    public class ChatSettingsHelper : IChatSettingsHelper
    {
        IRepository<Airport> _airportRepo;
        public ChatSettingsHelper(IRepository<Airport> airportRepo)
        {
            _airportRepo = airportRepo;
        }

        public ScheduleSettings CreateScheduleSettings(Schedule schedule)
        {
            switch (schedule.Type)
            {
                case ScheduleIntervalType.Hour:
                    return new ScheduleSettings(schedule.Id.ToString(), schedule.IsActive, schedule.Interval);
                case ScheduleIntervalType.Day:
                    return new ScheduleSettings(schedule.Id.ToString(), schedule.IsActive, schedule.Interval, schedule.Hour, schedule.Minute);
                case ScheduleIntervalType.Custom:
                    return new ScheduleSettings(schedule.Id.ToString(), schedule.IsActive, schedule.CustomCronSchedule);
                default:
                    throw new NotSupportedException();
            }
        }

        public Dictionary<Days, HashSet<byte>> GetDayTimeSlots(IEnumerable<FlyDaySettings> flyDaySettings)
        {
            var res = new Dictionary<Days, HashSet<byte>>(7);

            var allVals = Enum.GetValues(typeof(Days)).Cast<Days>();

            foreach (var v in allVals)
                res.Add(v, new HashSet<byte>());

            foreach (var set in flyDaySettings)
            {
                var hs = res[set.Day];

                for (var i = set.AllowedHourStart; i <= set.AllowedHourEnd; i++)
                    hs.Add(i);
            }

            return res;
        }

        public async Task<IEnumerable<Airport>> GetDestinationAirports(ChatSearchSettings searchSettings)
        {
            var airportIds = new HashSet<ObjectId>();
            foreach (var group in searchSettings.SearchGroups.Where(sg => sg.IsActive))
                foreach (var a in group.AirportsIds)
                    airportIds.Add(a);


            var data = await _airportRepo.Set.FindAsync(x => airportIds.Contains(x.Id));

            return await data.ToListAsync();
        }
    }
}
