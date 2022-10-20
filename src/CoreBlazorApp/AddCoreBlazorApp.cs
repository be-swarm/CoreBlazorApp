using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;

using Blazored.SessionStorage;

using Microsoft.Extensions.DependencyInjection;

namespace BeSwarm.CoreBlazorApp;

public static class CoreBlazorApp
{
	public static IServiceCollection AddCoreBlazorApp(this IServiceCollection services)
	{

		services.AddScoped<ErrorDialogService>();
		services.AddScoped<ConfirmDialogService>();
		services.AddScoped<IThemeService, ThemeService>();
		services.AddBeSwarmWebApiClient();
		services.AddBlazoredSessionStorage();
		services.AddMudServices();
		return services;
	}
}