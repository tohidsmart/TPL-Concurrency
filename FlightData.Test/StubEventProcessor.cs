using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FlightData.Entities;

namespace FlightData.Test
{
    public class StubEventProcessor : IEventProcessor
    {
        public BufferBlock<IList<string>> GetBufferBlock()
        {
            throw new NotImplementedException();
        }

        public void PostFile(string file, ITargetBlock<string> batchReadBlock)
        {
            throw new NotImplementedException();
        }

        public Task<ActionBlock<IList<RootData>>> Process(BufferBlock<IList<string>> batchBlock)
        {
            throw new NotImplementedException();
        }
    }
}
