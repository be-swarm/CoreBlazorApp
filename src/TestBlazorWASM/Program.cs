using BeSwarm.CoreBlazorApp;
using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApi;

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

builder.Services.AddCoreBlazorApp();
// inject blazor specific login service
builder.Services.AddScoped<ILoginBeSwarmService, BlazorLoginBeSwarmService>();
//builder.Services.AddScoped<ISessionPersistence,SessionPersistenceToSessionWeb>();
builder.Services.AddScoped<ISessionPersistence, SessionPersistenceToLocalWeb>();
builder.Services.AddSingleton<ISecureConfig, SecureConfig>();
builder.Services.AddScoped<ICryptoService, CryptoFromJS>();

await builder.Build().RunAsync();
