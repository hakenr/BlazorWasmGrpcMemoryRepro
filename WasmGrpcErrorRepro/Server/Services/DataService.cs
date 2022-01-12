using WasmGrpcErrorRepro.Shared;

namespace WasmGrpcErrorRepro.Server.Services
{
  public class DataService : IDataService
  {
    public Task<IEnumerable<int>> GetDataAsync(DataRequest request) =>
      Task.FromResult(Enumerable.Repeat(-1, request.Count));
  }
}
