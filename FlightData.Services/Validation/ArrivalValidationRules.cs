using System;
using System.Collections.Generic;
using System.Linq;
using FlightData.Entities;

namespace FlightData.Services
{
    public class ArrivalValidationRules : BaseValidationRules<Arrival>
    {

        public override List<Func<Arrival, bool>> GetRules()
        {
            var baseRules = base.GetRules().ToList();
            IEnumerable<Func<Arrival, bool>> rules = new List<Func<Arrival, bool>>
            {
                new Func<Arrival, bool>(d=>Convert.ToInt16(d.passenger)>0)
            };

            baseRules.AddRange(rules);

            return baseRules.ToList();
        }
    }
}