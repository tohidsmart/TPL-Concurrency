using FlightData.Entities;

namespace FlightData.Services
{
    public interface IValidationService
    {
        bool ValidateDeparture(Departure departure);
        bool ValidateArrival(Arrival arrival);
    }
}