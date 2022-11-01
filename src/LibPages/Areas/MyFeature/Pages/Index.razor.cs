using BeSwarm.CoreBlazorApp.Components;
using BeSwarm.Validator;
using BeSwarm.WebApiClient.Models;
using BeSwarm.WebApiClient.Referentials;

using LibPages.Models;

using Microsoft.AspNetCore.Components;

using System.Globalization;
using BeSwarm.WebApiClient;

namespace LibPages.Areas.MyFeature.Pages;

public partial class Index : IDisposable
{
	 Test model = new();
	 string status = "";
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Inject] ErrorDialogService ErrorDialogService { get; set; } = default!;
	private readonly System.Resources.ResourceManager _rm = new("LibPages.Resources.App", System.Reflection.Assembly.GetExecutingAssembly());

	private readonly ValidateContext _validatorContext = new(false);
	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		if (FirstTime)
		{
			Session.EnvironmentHasChanged += async (ChangeEvents e) => await Refresh(e);
			await Refresh(0);
		}
		
		CultureInfo.CurrentUICulture = new CultureInfo(Session.Lang);
		CultureInfo.CurrentCulture = new CultureInfo(Session.Lang);
		await base.OnAfterRenderAsync(FirstTime);
	}
	private async Task Refresh(ChangeEvents e)
	{
		model.Date = DateTime.Now;
		model.Age = 67;
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
	async Task ShowDialog()
	{
		await confirmservice.Show("Question", "Delete items", "YES", "NO", OnYes, OnNo);
	}
	private async Task OnYes()
	{
		status = "YES";
	}
	private async Task OnNo()
	{
		status = "NO";
	}

	private async Task CallSomeWebApi()
	{
		var httpclient = Session.GetUserHttpClient();
		BeSwarm.WebApiClient.Referentials.Referentials refs = new("", httpclient);
		try
		{
			BeSwarm.WebApiClient.Referentials.ReferentialItemListResultAction result = await refs.GetReferentialListAsync(Session.UserToken, "diets");
		}
		catch (Exception e)
		{
			// show error if needed
			ResultAction err = Session.GetInternalErrorFromException(e);
		}

	}

	void IDisposable.Dispose()
	{
		Session.EnvironmentHasChanged -= async (ChangeEvents e) => await Refresh(e);
	}
}


