using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;



namespace BeSwarm.CoreBlazorApp.Pages;

public partial class Login
{
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Inject] NavigationManager NavigationManager { get; set; } = default!;
	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{

		if (FirstTime)
		{
			// is callback ?
			var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
			// return of oauth ?
			if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var param))
			{
				await Session.CreateWebApiSession(NavigationManager.Uri);
			}

		}

	}

}


