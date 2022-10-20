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

			//
			// Configure oauth and add CoreBlazorApp
			//
			ConfigOauth.UserSwarm = "testdev";
			ConfigOauth.ClientSecret = "MySecret";
			ConfigOauth.Applicationid = "fc824c2d-5ce9-4f81-8199-4b76186d474d.77c8cdea-480f-47b6-8d20-67c6a39c9a9c.testdev";
			ConfigOauth.Serviceentrypoint = "https://dev.user.beswarm.net";
			ConfigOauth.CallbackUri = "com.beswarm.testmauiblazor://";
			builder.Services.AddCoreBlazorApp();
			// inject maui specific login service
			builder.Services.AddScoped<ILoginBeSwarmService, MauiAuthenticator>();


			builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");

			builder.Services.AddMauiBlazorWebView();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif

			OperatingSystem os = Environment.OSVersion;
			PlatformID pid = os.Platform;

			return builder.Build();
		}
	}
}