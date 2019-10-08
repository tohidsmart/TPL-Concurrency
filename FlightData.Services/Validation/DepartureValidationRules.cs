using System;
using System.Collections.Generic;
using System.Linq;
using FlightData.Entities;

namespace FlightData.Services
{
    public class DepartureValidationRules : BaseValidationRules<Departure>
    {
        public override List<Func<Departure, bool>> GetRules()
        {
            var baseRules = base.GetRules().ToList();
            IEnumerable<Func<Departure, bool>> rules = new List<Func<Departure, bool>>
            {
                // room for improvement : put the human-reaable logic in config file and read it from there
                new Func<Departure, bool>(d=>!string.IsNullOrEmpty(d.destination))
            };

            baseRules.AddRange(rules);

            return baseRules.ToList();
        }
    }
}
