using ProtoBuf.Grpc;
using System.ServiceModel;

namespace WasmGrpcErrorRepro.Shared
{
  [ServiceContract]
  public interface IDataService
  {
    [OperationContract]
    Task<IEnumerable<int>> GetDataAsync(DataRequest request);
  }
}
