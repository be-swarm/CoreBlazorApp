using Microsoft.AspNetCore.Components;

namespace BeSwarm.CoreBlazorApp.Services;



public partial class ThemeMudBlazor : ITheme
{
	private readonly MudTheme _theme = new();
	bool darkmode = false;
	public MudThemeProvider mref = default!;
	[Inject] public IThemeService Service { get; set; } = default!;

	public void SetDarkMode(bool darkmode)
	{
		this.darkmode = darkmode;
		StateHasChanged();
	}

	protected override async Task OnInitializedAsync()
	{
		Service.child = this;
		await base.OnInitializedAsync();
	}
}


