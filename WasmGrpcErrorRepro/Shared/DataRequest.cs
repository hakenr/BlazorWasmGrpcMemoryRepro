using ProtoBuf;

namespace WasmGrpcErrorRepro.Shared
{
  [ProtoContract]
  public class DataRequest
  {
    [ProtoMember(1)]
    public int Count { get; set; }
  }
}
