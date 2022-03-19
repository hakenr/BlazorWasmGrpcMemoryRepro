using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WasmGrpcErrorRepro.Client
{
	public class GrpcSingleCallInterceptor : Interceptor
	{
		public GrpcSingleCallInterceptor()
		{
			Console.WriteLine("GrpcSingleCallInterceptor:ctor");
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
					TRequest request,
					ClientInterceptorContext<TRequest, TResponse> context,
					AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			Console.WriteLine("AsyncUnaryCall_Start");
			var call = continuation(request, context);
			Console.WriteLine("AsyncUnaryCall_Start_postCall");

			return new AsyncUnaryCall<TResponse>(HandleResponseAsync(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
		}

		private readonly SemaphoreSlim callLock = new SemaphoreSlim(1, 1);
		private async Task<TResponse> HandleResponseAsync<TResponse>(Task<TResponse> responseTask)
		{
			Console.WriteLine($"GrpcSingleCallInterceptor_pre:callLock.CurrentCount={callLock.CurrentCount}");
			await callLock.WaitAsync();
			Console.WriteLine($"GrpcSingleCallInterceptor_post:callLock.CurrentCount={callLock.CurrentCount}");
			try
			{
				Console.WriteLine("GrpcSingleCallInterceptor:HandleResponseAsync");
				return await responseTask;
			}
			finally
			{
				Console.WriteLine($"GrpcSingleCallInterceptor_Release_pre:callLock.CurrentCount={callLock.CurrentCount}");
				callLock.Release();
				Console.WriteLine($"GrpcSingleCallInterceptor_Release_post:callLock.CurrentCount={callLock.CurrentCount}");
			}
		}
	}
}
