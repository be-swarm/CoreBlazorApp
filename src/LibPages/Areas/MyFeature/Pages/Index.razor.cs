using BeSwarm.CoreBlazorApp.Components;
using BeSwarm.Validator;
using BeSwarm.WebApi.Models;
using LibPages.Models;

using Microsoft.AspNetCore.Components;

using System.Globalization;
using LibPages.Resources;

namespace LibPages.Areas.MyFeature.Pages;

public partial class Index : IDisposable
{
	 Test model = new();
	 string status = "";
	[CascadingParameter] BeSwarmEnvironment Session { get; set; } = default!;
	[Inject] ErrorDialogService ErrorDialogService { get; set; } = default!;
	private readonly System.Resources.ResourceManager _rm = new("LibPages.Resources.App", System.Reflection.Assembly.GetExecutingAssembly());
	private readonly ValidateContext _validatorContext = new(false);
	private AppRes app = new();
	protected override async Task OnAfterRenderAsync(bool FirstTime)
	{
		if (FirstTime)
		{   
			Session.EnvironmentHasChanged += async (ChangeEvents e) => await Refresh(e);
			app.Culture = new CultureInfo(Session.Lang); 
			await Refresh(0);
		}
		
		CultureInfo.CurrentUICulture = new CultureInfo(Session.Lang);
		CultureInfo.CurrentCulture = new CultureInfo(Session.Lang);
		await base.OnAfterRenderAsync(FirstTime);
	}
	private async Task Refresh(ChangeEvents e)
	{
		_validatorContext.Culture = new CultureInfo(Session.Lang);
		if (e == ChangeEvents.Lang)
		{
			app.Culture = new CultureInfo(Session.Lang);
		}
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
		BeSwarm.WebApi.Referentials.Referentials refs = new("", httpclient);
		try
		{
			BeSwarm.WebApi.Referentials.ReferentialItemListResultAction result = await refs.GetReferentialListAsync(Session.UserToken, "diets");
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


