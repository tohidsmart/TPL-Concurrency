using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlightData.Entities;
using FlightData.Services;


namespace FlightData.Services
{
    public class ValidationService : IValidationService
    {
        private BaseValidationRules<Departure> departureRules;
        private BaseValidationRules<Arrival> arrivalRules;
        public ValidationService(BaseValidationRules<Departure> departureRules,
                                   BaseValidationRules<Arrival> arrivalRules)
        {
            this.departureRules = departureRules;
            this.arrivalRules = arrivalRules;
        }

        public bool ValidateArrival(Arrival arrival)
        {
            return arrivalRules.GetRules().All(r => r.Invoke(arrival));
        }

        public bool ValidateDeparture(Departure departure)
        {
            return departureRules.GetRules().All(r => r.Invoke(departure));
        }
    }
}
