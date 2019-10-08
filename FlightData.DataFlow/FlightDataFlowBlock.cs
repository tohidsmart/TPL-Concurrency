using FlightData.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlightData.Services;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using static FlightData.Entities.Model.DataEvent;
using FlightData.Services.Log;
using FlightData.Entities.Common;
using System.Diagnostics;

namespace FlightData.DataFlow
{
    public class FlightDataFlowBlock : IDataFlow
    {

        IValidationService validationService;
        ILogger<FlightDataFlowBlock> logger;

        public FlightDataFlowBlock(IValidationService validationService, ILogger<FlightDataFlowBlock> logger)
        {
            this.logger = logger;
            this.validationService = validationService;
        }


        public BufferBlock<IList<string>> GetReadFileBufferBlock()
        {
            return new BufferBlock<IList<string>>();
        }

        public TransformBlock<IList<string>, IList<RootData>> ReadDeserializeJsonfiles(string rawFolder)
        {

            var readFileBlock = new TransformBlock<IList<string>, IList<RootData>>(files =>
           {
               IList<RootData> rootDatas = new List<RootData>();
               foreach (var file in files)
               {
                   try
                   {
                       File.Copy(file, Path.Combine(rawFolder, Path.GetFileName(file)));
                       string content = File.ReadAllText(file);
                       var data = JsonConvert.DeserializeObject<RootData>(content);
                       rootDatas.Add(data);
                   }
                   catch (Exception ex)
                   {

                       throw;
                   }
               }
               return rootDatas;
           }, new ExecutionDataflowBlockOptions
           {
               MaxDegreeOfParallelism = 10,
               EnsureOrdered = true
           });
            return readFileBlock;
        }

        public ActionBlock<IList<RootData>> TransformEventsData(string curatedFolder, string exceptionFolder)
        {
            var transformBlock = new ActionBlock<IList<RootData>>(async allEvents =>
            {
                LogData logData = new LogData();
                int failedEvents = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var events in allEvents)
                {

                    foreach (var eventData in events.arrivals)
                    {
                        if (validationService.ValidateArrival(eventData))
                        {
                            await WriteEventDataToFile(curatedFolder, DateTime.Now.Ticks.ToString(), eventData);
                        }
                        else
                        {
                            logData.FailedEventId.Add(new KeyValuePair<Guid, string>(eventData.Id, "Arrival"));
                            failedEvents++;
                            await WriteEventDataToFile(exceptionFolder, DateTime.Now.Ticks.ToString(), eventData);
                        }
                    }
                    foreach (var eventData in events.departures)
                    {
                        if (validationService.ValidateDeparture(eventData))
                        {
                            await WriteEventDataToFile(curatedFolder, DateTime.Now.Ticks.ToString(), eventData);
                        }
                        else
                        {

                            logData.FailedEventId.Add(new KeyValuePair<Guid, string>(eventData.Id, "Departure"));
                            failedEvents++;
                            await WriteEventDataToFile(exceptionFolder, DateTime.Now.Ticks.ToString(), eventData);
                        }
                    }
                }
                stopwatch.Stop();

                if (allEvents.Any(rd => rd.departures.Any() || rd.arrivals.Any()))
                {
                    logData.ProcessedEvents.Add("Arrival", allEvents.Sum(rd => rd.arrivals.Count));
                    logData.ProcessedEvents.Add("Departure", allEvents.Sum(rd => rd.departures.Count));
                    logData.Duration = stopwatch.ElapsedMilliseconds;
                    logData.FailedEventsCount = failedEvents;
                    logger.LogInformation(new EventId(), LogBuilder.SerializeLog(logData));
                }

            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 3,
            });

            return transformBlock;
        }

        private async Task WriteEventDataToFile(string path, string tick, BaseEvent eventData)
        {
            string fullPath = Path.Combine(path, eventData.eventType);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(Path.Combine(path, eventData.eventType));
            string content = JsonConvert.SerializeObject(eventData, Formatting.Indented);
            string fileName = $"{eventData.eventType}-{tick}.json";
            await File.WriteAllTextAsync(Path.Combine(fullPath, fileName), content);
        }

    }

}
