using FlightData.DataFlow;
using FlightData.Entities;
using FlightData.Services;
using FlightData.Services.Utility;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace FlightData
{
    public class FlightEventProcessor : IEventProcessor
    {
        private readonly IDataFlow dataFlowBlockProvider;
        private readonly IValidationService validationService;
        private readonly IConfiguration configuration;

        public FlightEventProcessor(IConfiguration configuration, IDataFlow dataFlowBlockProvider,
                                IValidationService validationService)
        {
            this.dataFlowBlockProvider = dataFlowBlockProvider;
            this.validationService = validationService;
            this.configuration = configuration;
        }

        public void PostFile(string file, ITargetBlock<string> batchReadBlock)
        {
            batchReadBlock.Post(file);
        }

        public BufferBlock<IList<string>> GetBufferBlock()
        {
            return dataFlowBlockProvider.GetReadFileBufferBlock();
        }

        public async Task<ActionBlock<IList<RootData>>> Process(BufferBlock<IList<string>> batchBlock)
        {

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var readDataContentBlock = dataFlowBlockProvider.ReadDeserializeJsonfiles(PathUtility.RawFolder);
            var transformDataContentBlock = dataFlowBlockProvider.TransformEventsData(PathUtility.CuratedFolder, PathUtility.ExceptionFolder);

            while (await batchBlock.OutputAvailableAsync())
            {
                readDataContentBlock.Post(batchBlock.Receive());
            }

            batchBlock.LinkTo(readDataContentBlock, linkOptions);


            readDataContentBlock.LinkTo(transformDataContentBlock, linkOptions);


            await batchBlock.Completion.ContinueWith(delegate { readDataContentBlock.Complete(); });

            await readDataContentBlock.Completion.ContinueWith(delegate { transformDataContentBlock.Complete(); });

            return transformDataContentBlock;

        }




    }
}
