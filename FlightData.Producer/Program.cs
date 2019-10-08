using FlightData.Entities;
using FlightData.Services.Utility;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace FlightData.Producer
{
    class Program
    {



        static void Main(string[] args)
        {


            RootData rootData = new RootData();

            string inputFolder = PathUtility.InputFolder;
            Console.WriteLine("Producer generating files", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.WriteLine($"Input folder : { inputFolder}");
            Console.WriteLine("Press any key to continue", Console.ForegroundColor = ConsoleColor.Green);
            Console.WriteLine("Press 'q' to terminate",Console.ForegroundColor=ConsoleColor.Red);

            while (Console.ReadKey().KeyChar != 'q')
            {
                int fileloopCounter = new Random().Next(1, 10);
                int dataLoopCounter = new Random().Next(1, 20);
                int invalidPassenger = new Random().Next(-10, 0);
                int validPassengeer = new Random().Next(1, 300);
                string inValidFLightCode = "12AKBLahBLah";
                string validFlightCode = "AKBLahBLah";


                for (int i = 1; i <= fileloopCounter; i++)
                {
                    for (int j = 1; j < dataLoopCounter; j++)
                    {
                        Departure departure = new Departure();
                        departure.Id = Guid.NewGuid();
                        departure.destination = "Destination";
                        departure.timeStamp = DateTime.Now;
                        departure.eventType = "Departure";

                        Arrival arrival = new Arrival();
                        arrival.Id = Guid.NewGuid();
                        arrival.timeStamp = DateTime.Now.AddHours(12);
                        arrival.delayed = DateTime.Now.ToString("HH:mm");
                        arrival.eventType = "Arrival";

                        if (j % 2 == 0)
                        {

                            departure.flight = validFlightCode;
                            departure.passenger = validPassengeer.ToString();

                            arrival.flight = validFlightCode;
                            arrival.passenger = validPassengeer.ToString();

                        }
                        else
                        {
                            departure.flight = inValidFLightCode;
                            departure.passenger = invalidPassenger.ToString();

                            arrival.flight = inValidFLightCode;
                            arrival.passenger = invalidPassenger.ToString();
                        }
                        rootData.arrivals.Add(arrival);
                        rootData.departures.Add(departure);
                    }

                    string path = Path.Combine(inputFolder, $"{DateTime.Now.Ticks.ToString()}.json");
                    File.WriteAllText(path, JsonConvert.SerializeObject(rootData, Formatting.Indented));
                    Console.WriteLine(path,Console.ForegroundColor=ConsoleColor.White);
                    rootData.arrivals.Clear();
                    rootData.departures.Clear();
                }

                Console.WriteLine("Producer is in Idle state",Console.ForegroundColor=ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue or 'q' to exit",Console.ForegroundColor=ConsoleColor.Green);
                Console.ReadKey();
            }

        }


    }
}
