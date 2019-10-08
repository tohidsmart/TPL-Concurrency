using FlightData.DataFlow;
using FlightData.Entities;
using FlightData.Services;
using FlightData.Services.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FlightData.Test
{
    [TestClass]
    public class DataFlowBlockTests
    {
        private IDataFlow dataFlow;
        private BaseValidationRules<Departure> departureValidationService;
        private BaseValidationRules<Arrival> arrivalValidationService;
        private IValidationService validationService;

        public string InputFolder
        {
            get
            {
                return PathUtility.GetSubFolderPath(@"FlightData.Test\SampleData\Input");
            }
        }

        public string RAWFolder
        {
            get
            {
                return PathUtility.GetSubFolderPath(@"FlightData.Test\SampleData\RAW");
            }
        }

        public string ExceptionFolder
        {
            get
            {
                return PathUtility.GetSubFolderPath(@"FlightData.Test\SampleData\Exception");
            }
        }

        public string CuratedFolder
        {
            get
            {
                return PathUtility.GetSubFolderPath(@"FlightData.Test\SampleData\Curated");
            }
        }


        public DataFlowBlockTests()
        {
            departureValidationService = new DepartureValidationRules();
            arrivalValidationService = new ArrivalValidationRules();
            validationService = new ValidationService(departureValidationService, arrivalValidationService);
            var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<FlightDataFlowBlock>();
            dataFlow = new FlightDataFlowBlock(validationService, logger);
        }
        [TestMethod]
        public void FlightDataFlow_PostFile_CopiedToRawFolder()
        {
            var bufferBlock = dataFlow.GetReadFileBufferBlock();
            DataflowLinkOptions options = new DataflowLinkOptions { PropagateCompletion = true };
            var readBlock = dataFlow.ReadDeserializeJsonfiles(RAWFolder);
            bufferBlock.Post(new List<string>(Directory.GetFiles(InputFolder)));
            bufferBlock.LinkTo(readBlock, options);
            bufferBlock.Complete();
            bufferBlock.Completion.ContinueWith(delegate { readBlock.Complete(); });
            readBlock.Completion.Wait(5000);

            var inputFilesCount = Directory.GetFiles(InputFolder).Length;
            var rawFilesCount = Directory.GetFiles(RAWFolder).Length;
            Directory.GetFiles(RAWFolder).ToList().ForEach(file => File.Delete(file));
            Assert.AreEqual(inputFilesCount, rawFilesCount);
        }

        [TestMethod]
        public void FlightDataFlow_PostFile_FileReadConvertedToObjects()
        {
            var bufferBlock = dataFlow.GetReadFileBufferBlock();
            DataflowLinkOptions options = new DataflowLinkOptions { PropagateCompletion = true };
            var readBlock = dataFlow.ReadDeserializeJsonfiles(RAWFolder);
            bufferBlock.LinkTo(readBlock, options);
            bufferBlock.Completion.ContinueWith(delegate { readBlock.Complete(); });

            bufferBlock.Post(new List<string>(Directory.GetFiles(InputFolder)));
            bufferBlock.Complete();
            readBlock.Completion.Wait(5000);

            IList<RootData> data = readBlock.Receive();
            Directory.GetFiles(RAWFolder).ToList().ForEach(file => File.Delete(file));
            Assert.IsNotNull(data);
            Assert.AreNotEqual(0, data.Sum(rdata => rdata.arrivals.Count));
            Assert.AreNotEqual(0, data.Sum(rdata => rdata.departures.Count));
        }

        [TestMethod]
        public void FlightDataFLow__PostFile_FileReadConvertedWrittenToDestination()
        {
            var bufferBlock = dataFlow.GetReadFileBufferBlock();
            DataflowLinkOptions options = new DataflowLinkOptions { PropagateCompletion = true };
            var readBlock = dataFlow.ReadDeserializeJsonfiles(RAWFolder);
            var transformBLock = dataFlow.TransformEventsData(CuratedFolder, ExceptionFolder);
            bufferBlock.LinkTo(readBlock, options);
            readBlock.LinkTo(transformBLock, options);

            bufferBlock.Post(new List<string>(Directory.GetFiles(InputFolder)));
            bufferBlock.Complete();
           
            bufferBlock.Completion.ContinueWith(delegate { readBlock.Complete(); });
            readBlock.Completion.ContinueWith(delegate { transformBLock.Complete(); });

            transformBLock.Completion.Wait(5000);

            var allTransformedFilesCount = Directory.GetFiles(CuratedFolder, "*.json", SearchOption.AllDirectories).Count() +
            Directory.GetFiles(ExceptionFolder, "*.json", SearchOption.AllDirectories).Count();

            Assert.AreNotEqual(0, allTransformedFilesCount);

            Directory.GetFiles(CuratedFolder, "*.json", SearchOption.AllDirectories).ToList().ForEach(file => File.Delete(file));
            Directory.GetFiles(ExceptionFolder, "*.json", SearchOption.AllDirectories).ToList().ForEach(file => File.Delete(file));
            Directory.GetFiles(RAWFolder).ToList().ForEach(file => File.Delete(file));

        }
    }
}
