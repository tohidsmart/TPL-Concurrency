using System;
using System.Collections.Generic;
using System.Text;

namespace FlightData.Entities
{
    public class RootData
    {
        public List<Departure> departures { get; set; }

        public List<Arrival> arrivals { get; set; }

        public RootData()
        {
            departures = new List<Departure>();
            arrivals = new List<Arrival>();
        }
    }
}
