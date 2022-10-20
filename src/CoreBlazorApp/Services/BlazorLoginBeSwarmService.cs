using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Services;
public class BlazorLoginBeSwarmService : ILoginBeSwarmService
{
	NavigationManager NavigationManager { get; set; }
	public BlazorLoginBeSwarmService(NavigationManager navmanager)
	{
		NavigationManager = navmanager;
	}
	public async Task<LoginAction> Login(string url, string callbackurl)
	{
		NavigationManager.NavigateTo(url);
		return new LoginAction(); // nothing todo
	}
}
