using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components;



public partial class SetDarkTheme
{
	bool darkmode = false;

	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;

	bool ThemeDark { get => darkmode; set { darkmode = value; _ = Session.SetDarkMode(value); } }

	protected override void OnAfterRender(bool FirstTime)
	{
		if (FirstTime)
		{
			darkmode = Session.GetDarkMode;
			StateHasChanged();
		}
	}


}





