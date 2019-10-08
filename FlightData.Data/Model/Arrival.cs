using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightData.Entities
{
    public class Arrival :BaseEvent
    {
        public string delayed;

        public override BaseEvent GetEventObject(string eventData)
        {
            return JsonConvert.DeserializeObject<Arrival>(eventData);
        }
    }
}
