using BeSwarm.CoreBlazorApp;
using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;

using Microsoft.AspNetCore.Localization;

using System.Globalization;

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
//
// Configure oauth and add CoreBlazorApp
//
//ConfigOauth.userswarm = "testdev";
ConfigOauth.ClientSecret = "MySecret";
ConfigOauth.Applicationid = "fc824c2d-5ce9-4f81-8199-4b76186d474d.c6f4141b-5085-461e-b4d1-b7aa4e85bbfb.testdev";
ConfigOauth.Serviceentrypoint = "https://dev.user.BeSwarm.net";
builder.Services.AddCoreBlazorApp();
// inject blazor specific login service
builder.Services.AddScoped<ILoginBeSwarmService, BlazorLoginBeSwarmService>();







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
