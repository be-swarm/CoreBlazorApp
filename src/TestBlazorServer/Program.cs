using BeSwarm.CoreBlazorApp;
using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;

using Microsoft.AspNetCore.Localization;

using System.Globalization;

using TestBlazorServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//	var supportedCultures = new List<CultureInfo>()
//				{
//					new CultureInfo("en"),
//					new CultureInfo("fr")
//				};
//	options.DefaultRequestCulture = new RequestCulture("fr");
//	options.SupportedCultures = supportedCultures;
//	options.SupportedUICultures = supportedCultures;
//});

builder.Services.AddControllers();
builder.Services.AddCoreBlazorApp();
// inject blazor specific login service
builder.Services.AddScoped<ILoginBeSwarmService, BlazorLoginBeSwarmService>();
//builder.Services.AddScoped<ISessionPersistence,SessionPersistenceToSessionWeb>();
builder.Services.AddScoped<ISessionPersistence, SessionPersistenceToLocalWeb>();
builder.Services.AddSingleton<ISecureConfig, SecureConfig>();







var app = builder.Build();
app.UseRequestLocalization(new RequestLocalizationOptions()
	.AddSupportedCultures(new[] { "en", "fr" })
	.AddSupportedUICultures(new[] { "en", "fr" }));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
