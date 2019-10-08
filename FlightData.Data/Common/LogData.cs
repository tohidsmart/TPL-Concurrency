using System;
using System.Collections.Generic;
using System.Text;
using static FlightData.Entities.Model.DataEvent;

namespace FlightData.Entities.Common
{
    public class LogData
    {
        public Dictionary<string, int> ProcessedEvents { get; set; }
        public long Duration { get; set; }
        public int FailedEventsCount { get; set; }
        public List<KeyValuePair<Guid, string>> FailedEventId { get; set; }

        public LogData()
        {
            ProcessedEvents = new Dictionary<string, int>();
            FailedEventId = new List<KeyValuePair<Guid, string>>();
        }

    }
}
