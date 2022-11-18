using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApi.Models;

using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

using Newtonsoft.Json;

using System.Globalization;
using BeSwarm.WebApi;
using BeSwarm.WebApi.Services;
using BeSwarm.WebApi.Services;

namespace BeSwarm.CoreBlazorApp.Components
{

	public enum ChangeEvents
	{
		Init = 1,
		Lang = 2,
		Login = 3,
		Logout = 4,
		DarkMode = 5,
		AmPm = 6,

	}
	
	public partial class BeSwarmEnvironment : IAsyncDisposable
	{
		class SessionConfiguration
		{
			public string Lang { get; set; } = "FR";
			public bool AmPm { get; set; } = false;
			public bool IsDark { get; set; } = false;
			public string SessionWebApi { get; set; } = "";
			public string RouteAfterLogin { get; set; } = "/";   // only for blazor app

		}

		[Parameter] public RenderFragment ChildContent { get; set; } = default!;
		[Inject] private SessionWebApi SessionWebApi { get; set; } = default!;
		[Inject] private ISessionPersistence Persistence { get; set; } = default!;
		[Inject] private NavigationManager NavigationManager { get; set; } = default!;
		[Inject] private IJSRuntime JSRuntime { get; set; } = default!;
		[Inject] private IThemeService ThemeService { get; set; } = default!;
	    [Inject] private ILoginBeSwarmService LoginService { get; set; } = default!;
		[Inject] private ISecureConfig SecureConfig { get; set; } = default!;
		[Inject] private ICryptoService CryptoService { get; set; } = default!;

		[Inject] private ErrorDialogService ErrorDialogService { get; set; } = default!;
		private Task<IJSObjectReference>? _module;
		private const string ImportPath = "./_content/BeSwarm.CoreBlazorApp/coreblazorapp.js";
		private Task<IJSObjectReference> Module => _module ??= JSRuntime.InvokeAsync<IJSObjectReference>("import", ImportPath).AsTask();
		public bool IsBusy { get; private set; } = false;
		private SessionConfiguration Configuration { get; set; } = new SessionConfiguration();
		public event Action<ChangeEvents> EnvironmentHasChanged = default!;
		public event Action TraceHttpAdded = default!;
		protected override async Task OnAfterRenderAsync(bool FirstTime)
		{
			IsBusy = false;
			if (FirstTime)
			{
				IsBusy = true;
				await RestoreConfiguration();
				// init env
				//
				ThemeService.SetDarkMode(Configuration.IsDark);
				IsBusy = false;
				NotifyStateChanged(ChangeEvents.Init);
				NotifyStateChanged(ChangeEvents.Lang);
				NotifyStateChanged(ChangeEvents.AmPm);

				SessionWebApi.SessionHasChanged += async (SessionWebApiEvents e) => await SessionWebApiHasChanged(e);

			}
			
			await base.OnAfterRenderAsync(FirstTime);
		}
		//
		// something has changed in webapisession ?
		//
		public async Task SessionWebApiHasChanged(SessionWebApiEvents e)
		{
			if (e == SessionWebApiEvents.TokenRefreshed)
			{
				await SaveConfiguration();
			}
			if (e == SessionWebApiEvents.TraceHttpAdded)
			{
				TraceHttpAdded?.Invoke();
			}
			if (e == SessionWebApiEvents.Logout)  //webapisession has ended session ?
			{
				SaveConfiguration();
				NotifyStateChanged(ChangeEvents.Logout);
			}
			if (e == SessionWebApiEvents.Login)  //webapisession has started session ?
			{
				await SaveConfiguration();
				NotifyStateChanged(ChangeEvents.Login);
			}
		}
		//
		// accessors
		//
		public bool SessionIsActive => SessionWebApi.SessionIsActive;
		public string UserToken => SessionWebApi.UserToken;
		public ITraceHttp TracesHttp => SessionWebApi.TracesHttp;
		public async Task<ResultAction<List<ReferentialItemLang>>> GetReferentialItems(string referential, string lang) => await SessionWebApi.GetReferentialItems(referential, lang);
		public BeSwarmHttpClient GetUserHttpClient() => SessionWebApi.GetUserHttpClient();
		public ResultAction GetInternalErrorFromException(Exception e) => SessionWebApi.GetInternalErrorFromException(e);
		public CultureInfo CultureInfo => new CultureInfo(Configuration.Lang);
		public bool AmPm => Configuration.AmPm;
		public string Lang => Configuration.Lang;

		//
		// DarkMode
		//
		public async Task SetDarkMode(bool IsDark)
		{
			await RestoreConfiguration(); //if not yet restored and before OnAfterRenderAsync
			Configuration.IsDark = IsDark;
			await SaveConfiguration();
			ThemeService.SetDarkMode(Configuration.IsDark);
			NotifyStateChanged(ChangeEvents.DarkMode);
		}
		public bool GetDarkMode => Configuration.IsDark;
		//
		// Langs
		//
		public async Task SetLang(string codelang)

		{
			await RestoreConfiguration(); //if not yet restored and before OnAfterRenderAsync
			Configuration.Lang = codelang;
			if (codelang == "FR")
			{
				Configuration.AmPm = false;
			}
			else
			{
				Configuration.AmPm = true;
			}
			CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(codelang);
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(codelang);

			await SaveConfiguration();
		    NotifyStateChanged(ChangeEvents.Lang);
			NotifyStateChanged(ChangeEvents.AmPm);
		}
		//
		// AmPm time format
		//
		public async Task SetAmPm(bool ampm)

		{
			await RestoreConfiguration(); //if not yet restored and before OnAfterRenderAsync
			Configuration.AmPm = ampm;
			await SaveConfiguration();
			NotifyStateChanged(ChangeEvents.AmPm);
		}
		//
		// Persist session 
		//
		public async Task SaveConfiguration()
		{
			Configuration.SessionWebApi = SessionWebApi.SerializeCurrentSession();
			await Persistence.Save(SecureConfig.ApplicationId, await CryptoService.Encrypt(JsonConvert.SerializeObject(Configuration)));

		}
	
		public async Task RestoreConfiguration()
		{
			var json = await Persistence.Get(SecureConfig.ApplicationId);
			if (!string.IsNullOrEmpty(json))
			{
				var res = JsonConvert.DeserializeObject<SessionConfiguration>(await CryptoService.Decrypt(json));
				if (res is { })
				{
					Configuration = res;
					if (!string.IsNullOrEmpty(Configuration.SessionWebApi)) SessionWebApi.DeserializeCurrentSession(Configuration.SessionWebApi);
				}
			}

		}

		//
		// Oauth BeSwarm WebApi
		//
		//Get Tokens from callback url
		//
		public async Task<ResultAction> CreateWebApiSession(string uri)
		{
			// important restore session to retrieve statecode
			await RestoreConfiguration();
			var result = await SessionWebApi.GetUserTokens(uri);
			if (result.IsOk)
			{
				// save session
				await SaveConfiguration();
				NavigationManager.NavigateTo(Configuration.RouteAfterLogin);
			}
			else
				await ErrorDialogService.Show("error", $"{result.Status}   {result.Error.Description}");
			return result;
		}
		//
		// Open beswarm login url
		//
		public async Task Login(string routeafterlogin = "/")
		{   // only for blazor app
			string url = SessionWebApi.GetLoginUrl(SecureConfig.CallBackUri);
			Configuration.RouteAfterLogin = routeafterlogin;
			// save session
			await SaveConfiguration();  // important for saving statecode and RouteAfterLogin generated by GetLoginUrl
			var result = await LoginService.Login(url, SecureConfig.CallBackUri);
			if (result.Action == LoginActions.gettokens)
			{
				await CreateWebApiSession(result.CallBackUrl);
			}

		}
		public async Task Logout()
		{
			SessionWebApi.LogOut();
			await SaveConfiguration();
		}


		//
		// Misc
		//
		private void NotifyStateChanged(ChangeEvents e)
		{

			EnvironmentHasChanged?.Invoke(e);
		}

		async ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (SessionWebApi is { }) SessionWebApi.SessionHasChanged -= async (SessionWebApiEvents e) => await SessionWebApiHasChanged(e);
			if (_module != null)
			{
				var module = await _module;
				await module.DisposeAsync();
			}
		}
	}
}
