using BeSwarm.CoreBlazorApp;
using BeSwarm.CoreBlazorApp.Services;

/* Unmerged change from project 'TestMauiBlazor (net7.0-android)'
Before:
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebView.Maui;


using System.Globalization;
using BeSwarm.WebApiClient;
After:
using BeSwarm.WebApiClient;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebView.Maui;

using System.Globalization;
*/
using BeSwarm.WebApiClient;


namespace TestMauiBlazor
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

					builder.Services.AddCoreBlazorApp();
			// inject maui specific login service
			builder.Services.AddScoped<ILoginBeSwarmService, MauiAuthenticator>();
			builder.Services.AddScoped<ISessionPersistence, SessionPersistenceMaui>();
			builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
			builder.Services.AddMauiBlazorWebView();
			builder.Services.AddSingleton<ISecureConfig, SecureConfig>();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif

			OperatingSystem os = Environment.OSVersion;
			PlatformID pid = os.Platform;

			return builder.Build();
		}
	}
}