using System;
using System.Collections.Generic;
using System.Text;
using static FlightData.Entities.Model.DataEvent;

namespace FlightData.Entities
{
    public abstract class BaseEvent
    {
        public Guid Id { get; set; }
        public string flight { get; set; }
        public  string passenger { get; set; }
        public DateTime timeStamp { get; set; }
        public string eventType { get; set; }

        public abstract BaseEvent GetEventObject(string eventData);
    }
}
