using FlightData.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FlightData
{
    public interface IEventProcessor
    {

        Task<ActionBlock<IList<RootData>>> Process(BufferBlock<IList<string>> batchBlock);
        void PostFile(string file, ITargetBlock<string> batchReadBlock);
        BufferBlock<IList<string>> GetBufferBlock();



    }
}
