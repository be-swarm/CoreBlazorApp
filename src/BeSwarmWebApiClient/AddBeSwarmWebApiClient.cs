using BeSwarm.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeSwarm.WebApi;


public static class BeSwarmWebApiClient
{
	public static IServiceCollection AddBeSwarmWebApiClient(this IServiceCollection services)
	{
		services.AddHttpClient();
		services.AddScoped<SessionWebApi>();
		services.AddHttpClient();
     	services.AddHttpContextAccessor();
        services.AddScoped<ITraceHttp, TraceHttpInMemory>();
        return services;
	}
}