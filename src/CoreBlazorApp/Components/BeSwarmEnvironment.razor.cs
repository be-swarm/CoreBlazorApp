using BeSwarm.CoreBlazorApp.Services;
using BeSwarm.WebApiClient;
using BeSwarm.WebApiClient.Models;

using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using Newtonsoft.Json;

using System.Globalization;

namespace BeSwarm.CoreBlazorApp.Components
{

	public enum ChangeEvents
	{
		Init,
		Lang,
		Logout,
		DarkMode,
		Login
	}
	public enum Platforms
	{
		BlazorServer = 1,
		BlazorWasm = 2,
		Maui = 3
	}
	public partial class BeSwarmEnvironment : IAsyncDisposable
	{
		class SessionConfiguration
		{
			public string Lang { get; set; } = "FR";
			public bool IsDark { get; set; } = false;
			public string SessionWebApi { get; set; } = "";
			public string RouteAfterLogin { get; set; } = "/";   // only for blazor app
		}

		[Parameter] public RenderFragment ChildContent { get; set; } = default!;
		[Parameter] public Platforms Platform { get; set; } = default!;
		[Inject] public SessionWebApi SessionWebApi { get; private set; } = default!;
		[Inject] private ISessionPersistence Persistence { get; set; } = default!;
		[Inject] private NavigationManager NavigationManager { get; set; } = default!;
		[Inject] private IJSRuntime JSRuntime { get; set; } = default!;
		[Inject] private IThemeService ThemeService { get; set; } = default!;
		[Inject] private ILoginBeSwarmService LoginService { get; set; } = default!;

		private Task<IJSObjectReference>? _module;
		private const string ImportPath = "./_content/BeSwarm.CoreBlazorApp/coreblazorapp.js";
		private Task<IJSObjectReference> Module => _module ??= JSRuntime.InvokeAsync<IJSObjectReference>("import", ImportPath).AsTask();
		public bool IsBusy { get; private set; } = true;
		private SessionConfiguration Configuration { get; set; } = new SessionConfiguration();
		public event Action<ChangeEvents> EnvironmentHasChanged = default!;
	    protected override async Task OnAfterRenderAsync(bool FirstTime)
		{
			// assert platform is set
			if (Enum.IsDefined(typeof(Platforms), Platform) == false)
			{
				throw new Exception("Platform is not set usage ex: <BeSwarmEnvironment Platform=@Platforms.Maui> or <BeSwarmEnvironment platform=@Platforms.BlazorServer> or <BeSwarmEnvironment platform=@Platforms.BlazorWasm>");
			}
			if (FirstTime)
			{
				IsBusy = true;
				await RestoreConfiguration();
				//
				// init env
				//
				ThemeService.SetDarkMode(Configuration.IsDark);
				IsBusy = false;
				if (Platform == Platforms.BlazorWasm) await SetLang(Configuration.Lang);
				NotifyStateChanged(ChangeEvents.Init);

			}

		}
		//
		public async Task SetDarkMode(bool IsDark)
		{
			await RestoreConfiguration(); //if not yet restored and before OnAfterRenderAsync
			Configuration.IsDark = IsDark;
			await SaveConfiguration();
			ThemeService.SetDarkMode(Configuration.IsDark);
			NotifyStateChanged(ChangeEvents.DarkMode);
		}
		public bool GetDarkMode { get => Configuration.IsDark; }
		public async Task SetLang(string codelang)
		{
			await RestoreConfiguration(); //if not yet restored and before OnAfterRenderAsync
			Configuration.Lang = codelang;
			await SaveConfiguration();
			SetLang();
			NotifyStateChanged(ChangeEvents.Lang);
		}
		private void SetLang()
		{
			if (Platform == Platforms.Maui || Platform == Platforms.BlazorWasm)
			{
				CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(Configuration.Lang);
				CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Configuration.Lang);
			}
			else
			{
				var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
				var cultureEscaped = Uri.EscapeDataString(Configuration.Lang);
				var uriEscaped = Uri.EscapeDataString(uri);
				NavigationManager.NavigateTo(
					$"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
					forceLoad: true);
			}
		}

		public CultureInfo CultureInfo => new CultureInfo(Configuration.Lang);

		public string Lang => Configuration.Lang;
		//
		// Save session to web browser
		//
		public async Task SaveConfiguration()
		{
			if (Platform == Platforms.Maui) return; // only blazor app need save state
			Configuration.SessionWebApi = SessionWebApi.SerializeCurrentSession();
			await Persistence.Save("SessionBeSwarm", JsonConvert.SerializeObject(Configuration));

		}
		//
		// Restore session saved in web browser
		//
		public async Task RestoreConfiguration()
		{
			if (Platform == Platforms.BlazorServer || Platform == Platforms.BlazorWasm)  // only blazor app need restore state
			{
				var json = await Persistence.Get("SessionBeSwarm");
				if (!string.IsNullOrEmpty(json))
				{
					var res = JsonConvert.DeserializeObject<SessionConfiguration>(json);
					if (res is { })
					{
						Configuration = res;
						if (!string.IsNullOrEmpty(Configuration.SessionWebApi)) SessionWebApi.DeserializeCurrentSession(Configuration.SessionWebApi);
					}
				}
			}

		}
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
			NotifyStateChanged(ChangeEvents.Login);
			return result;
		}
		public async Task<ResultAction> CreateWebApiSession(Dictionary<string, string> keys)
		{
			// important restore session to retrieve statecode
			await RestoreConfiguration();
			var result = await SessionWebApi.GetUserTokens(keys);
			// save session
			await SaveConfiguration();
			NotifyStateChanged(ChangeEvents.Login);
			NavigationManager.NavigateTo(Configuration.RouteAfterLogin);
			return result;
		}
		//
		// Open beswarm login url
		//
		public async Task Login(string routeafterlogin = "/")
		{   // only for blazor app
			string url = SessionWebApi.GetLoginUrl();
			Configuration.RouteAfterLogin = routeafterlogin;
			// save session
			await SaveConfiguration();  // important for saving statecode and RouteAfterLogin generated by GetLoginUrl
			var result = await LoginService.Login(url, ConfigOauth.CallbackUri);
			if (result.Action == LoginActions.gettokens)
			{
				await CreateWebApiSession(result.CallBackUrl);
			}
			NotifyStateChanged(ChangeEvents.Login);

		}
		public async Task Logout()
		{
			SessionWebApi.LogOut();
			await SaveConfiguration();
			NotifyStateChanged(ChangeEvents.Login);
		}

		private void NotifyStateChanged(ChangeEvents e)
		{
			EnvironmentHasChanged?.Invoke(e);
		}
		async ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (_module != null)
			{
				var module = await _module;
				await module.DisposeAsync();
			}
		}
	}
}
