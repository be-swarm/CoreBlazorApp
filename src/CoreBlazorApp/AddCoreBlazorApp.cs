using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;

using Blazored.SessionStorage;
using Blazored.LocalStorage;

using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;

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
		services.AddBlazoredLocalStorage(config =>
		{
			config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
			config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
			config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
			config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
			config.JsonSerializerOptions.WriteIndented = false;
		});
		services.AddMudServices();
		return services;
	}
}