using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProtoBuf.Grpc.ClientFactory;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Meta;
using WasmGrpcErrorRepro.Client;
using WasmGrpcErrorRepro.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<GrpcSingleCallInterceptor>();

builder.Services.AddSingleton(sp =>
{
  var baseUri = sp.GetRequiredService<NavigationManager>().BaseUri;
  var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
  return GrpcChannel.ForAddress(baseUri,
    new GrpcChannelOptions() { HttpClient = new HttpClient(handler) });
});

//builder.Services.AddScoped(sp =>
//{
//  var channel = sp.GetRequiredService<GrpcChannel>();
//	return channel.CreateGrpcService<IDataService>();
//});

builder.Services.AddTransient<GrpcWebHandler>(provider => new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
builder.Services.AddSingleton<ClientFactory>(ClientFactory.Create());


builder.Services.AddCodeFirstGrpcClient<IDataService>((provider, options) =>
{
	var navigationManager = provider.GetRequiredService<NavigationManager>();
	var backendUrl = navigationManager.BaseUri;

	options.Address = new Uri(backendUrl);
})
	.ConfigurePrimaryHttpMessageHandler<GrpcWebHandler>()
	.AddInterceptor<GrpcSingleCallInterceptor>();

await builder.Build().RunAsync();
