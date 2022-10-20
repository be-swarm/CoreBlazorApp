using BeSwarm.CoreBlazorApp.Components;
using BeSwarm.Validator;

using LibPages.Models;

using Microsoft.AspNetCore.Components;

using System.Globalization;

namespace LibPages.Areas.MyFeature.Pages;

public partial class Index : IDisposable
{
	readonly Test model = new();
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Inject] ErrorDialogService ErrorDialogService { get; set; } = default!;
	private readonly System.Resources.ResourceManager _rm = new("LibPages.Resources.App", System.Reflection.Assembly.GetExecutingAssembly());

	private readonly ValidateContext _validatorContext = new(false);
	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		if (FirstTime)
		{
			Session.EnvironmentHasChanged += async (ChangeEvents e) => await Refresh(e);
		}
		CultureInfo.CurrentUICulture = new CultureInfo(Session.Lang);
		CultureInfo.CurrentCulture = new CultureInfo(Session.Lang);
		await base.OnAfterRenderAsync(FirstTime);
	}
	private async Task Refresh(ChangeEvents e)
	{
		if (e != ChangeEvents.DarkMode) StateHasChanged();
	}
	private void SubmitValidForm()
	{
		// do something with the model
	}
	private async Task ShowError()
	{
		await ErrorDialogService.Show("error", "This is an error message");
	}


	void IDisposable.Dispose()
	{
		Session.EnvironmentHasChanged -= async (ChangeEvents e) => await Refresh(e);
	}
}


