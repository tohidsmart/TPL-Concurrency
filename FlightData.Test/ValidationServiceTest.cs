using FlightData.DataFlow;
using FlightData.Entities;
using FlightData.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightData.Test
{
    [TestClass]
    public class ValidationServiceTest
    {

        private IDataFlow dataFlow;
        private BaseValidationRules<Departure> departureValidationService;
        private BaseValidationRules<Arrival> arrivalValidationService;
        private IValidationService validationService;

        public ValidationServiceTest()
        {
            departureValidationService = new DepartureValidationRules();
            arrivalValidationService = new ArrivalValidationRules();
            validationService = new ValidationService(departureValidationService, arrivalValidationService);
            dataFlow = new FlightDataFlowBlock(validationService, null);
        }

        [TestMethod]
        public void ArrivalFLightInvalid_ValidationReturnFalse()
        {
            string invlidFlightString = "{\"delayed\": \"21:22\",\"Id\": \"91a1d7f9-58ff-4c1e-b968-d9fb8bfb6268\", \"flight\": \"12AKBLahBLah\",\"passenger\": \"-6\", \"timeStamp\": \"0001-01-01T00:00:00\", \"eventType\": \"Arrival\"}";
            var invalidFlight = JsonConvert.DeserializeObject<Arrival>(invlidFlightString);
            bool isValid = validationService.ValidateArrival(invalidFlight);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ArrivalDataValid_ValidationReturnTrue()
        {
            string validFlightString = "{\"delayed\": \"21:22\",\"Id\": \"91a1d7f9-58ff-4c1e-b968-d9fb8bfb6268\", \"flight\": \"AKBLahBLah\",\"passenger\": \"223\", \"timeStamp\": \"0001-01-01T00:00:00\", \"eventType\": \"Arrival\"}";
            var validFlight = JsonConvert.DeserializeObject<Arrival>(validFlightString);
            bool isValid = validationService.ValidateArrival(validFlight);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void DepartureDataInValid_ValidationReturnFalse()
        {
            string invalidDepartureString = "{\"destination\": \"Destination\",\"Id\": \"58d8da8b-fc0f-44da-8f1f-c0ecf929bb10\", \"flight\": \"12AKBLahBLah\",\"passenger\": \"-2\", \"timeStamp\": \"2019-09-11T21:22:27.0191849+10:00\",\"eventType\": \"Departure\"}";
            var inValidFlight = JsonConvert.DeserializeObject<Departure>(invalidDepartureString);
            bool isValid = validationService.ValidateDeparture(inValidFlight);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void DepartureDataValid_ValidationReturnTrue()
        {
            string validDepartureString = "{\"destination\": \"Destination\",\"Id\": \"58d8da8b-fc0f-44da-8f1f-c0ecf929bb10\", \"flight\": \"AKBLahBLah\",\"passenger\": \"154\", \"timeStamp\": \"2019-09-11T21:22:27.0191849+10:00\",\"eventType\": \"Departure\"}";
           
            var validFlight = JsonConvert.DeserializeObject<Departure>(validDepartureString);
            bool isValid = validationService.ValidateDeparture(validFlight);

            Assert.IsTrue(isValid);
        }
    }
}
