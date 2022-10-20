using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components
{
	public partial class LoginButton : IDisposable
	{
		[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;


		protected override async Task OnAfterRenderAsync(bool FirstTime)
		{
			if (FirstTime)
			{
				Session.EnvironmentHasChanged += async (ChangeEvents e) => await InvokeAsync(StateHasChanged);
				StateHasChanged();
			}
			await base.OnAfterRenderAsync(FirstTime);
		}

		void IDisposable.Dispose()
		{
			Session.EnvironmentHasChanged -= async (ChangeEvents e) => await InvokeAsync(StateHasChanged);
		}
		private async Task Login()
		{
			await Session.Login();
		}
		private async Task Logout()
		{
			await Session.Logout();
		}

	}
}
