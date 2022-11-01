using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Components
{
	public partial class TracesHttp : IDisposable
	{
		[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;

		protected override async Task OnAfterRenderAsync(bool FirstTime)
		{
			if (FirstTime)
			{
				Session.TraceHttpAdded += async () => await OnRefresh();
				await OnRefresh();
			}

			await base.OnAfterRenderAsync(FirstTime);
		}
		private async Task OnRefresh()
		{
				StateHasChanged();
		}
		
		void IDisposable.Dispose()
		{
			Session.TraceHttpAdded -= async () => await OnRefresh();
		}
		

	}
}
