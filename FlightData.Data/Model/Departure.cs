using System;
using System.Collections.Generic;
using System.Text;

namespace FlightData.Entities
{
    public class Departure : BaseEvent
    {
        public string destination { get; set; }

        public override BaseEvent GetEventObject(string eventData)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Departure>(eventData);
        }

    }
}
