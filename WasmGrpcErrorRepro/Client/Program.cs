using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProtoBuf.Grpc.Client;
using WasmGrpcErrorRepro.Client;
using WasmGrpcErrorRepro.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(sp =>
{
  var baseUri = sp.GetRequiredService<NavigationManager>().BaseUri;
  var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
  return GrpcChannel.ForAddress(baseUri,
    new GrpcChannelOptions() { HttpClient = new HttpClient(handler) });
});

builder.Services.AddScoped(sp =>
{
  var channel = sp.GetRequiredService<GrpcChannel>();
  return channel.CreateGrpcService<IDataService>();
});

await builder.Build().RunAsync();
