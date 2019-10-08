using FlightData.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FlightData.DataFlow
{
    public interface IDataFlow
    {

        BufferBlock<IList<string>> GetReadFileBufferBlock();
        TransformBlock<IList<string>, IList<RootData>> ReadDeserializeJsonfiles(string rawFolder);
        ActionBlock<IList<RootData>> TransformEventsData(string curatedFolder, string exceptionFolder);

    }
}
