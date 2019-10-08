using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlightData.Entities;

namespace FlightData.Services
{
    public abstract class BaseValidationRules<T> where T : BaseEvent
    {
        public virtual List<Func<T, bool>> GetRules()
        {
            List<Func<T, bool>> rules = new List<Func<T, bool>>
            {
                new Func<T, bool>(d=> char.IsLetter(d.flight. First())),
                new Func<T, bool>(d=>Convert.ToInt16(d.passenger)>0)
            };
            return rules;
        }
    }
}
