using BeSwarm.CoreBlazorApp;
using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;

using System.Globalization;

using TestBlazorWASM;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new List<CultureInfo>()
	{
		new CultureInfo("en"),
		new CultureInfo("fr")
	};
	options.DefaultRequestCulture = new RequestCulture("fr");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});

//
// Configure oauth and add CoreBlazorApp
//
//ConfigOauth.userswarm = "testdev";
ConfigOauth.ClientSecret = "MySecret";
ConfigOauth.Applicationid = "fc824c2d-5ce9-4f81-8199-4b76186d474d.28ea0ff0-6bdf-4d73-ac8d-063ecbebf9a0.testdev";
ConfigOauth.Serviceentrypoint = "https://dev.user.BeSwarm.net";
builder.Services.AddCoreBlazorApp();
// inject blazor specific login service
builder.Services.AddScoped<ILoginBeSwarmService, BlazorLoginBeSwarmService>();


await builder.Build().RunAsync();
