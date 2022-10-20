using BeSwarm.CoreBlazorApp.Components;

using Beswarmwebapiclient;

using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace Beswarm.coremauiap
{
	public class WebBrowserAuthenticator : IdentityModel.OidcClient.Browser.IBrowser
	{
		private BeSwarmWebApiSession _session;

		public WebBrowserAuthenticator(BeSwarmWebApiSession session)
		{
			_session = session;
		}

		public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
		{
			try
			{
				WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(
					new Uri(options.StartUrl),
					new Uri(options.EndUrl));
				var status = await _session.CreateWebApiSession(result.Properties);
				var url = new RequestUrl(options.EndUrl).Create(new Parameters(result.Properties));
				return new BrowserResult
				{
					Response = url,
					ResultType = BrowserResultType.Success
				};
			}
			catch (TaskCanceledException)
			{
				return new BrowserResult
				{
					ResultType = BrowserResultType.UserCancel,
					ErrorDescription = "Login canceled by the user."
				};
			}
		}
	}

	public static class AuthClient
	{
		public static async Task<LoginResult> LoginAsync(BeSwarmWebApiSession session)
		{
			OidcClient oidcClient = new OidcClient(new OidcClientOptions
			{
				ProviderInformation = new()
				{
					AuthorizeEndpoint = session.SessionWebApi.GetLoginUrl(),
					TokenEndpoint = "https://????.???",  //required but not used
					KeySet = new(),
					IssuerName = "me"
				},
				RedirectUri = ConfigOauth.callbackuri,
				Browser = new WebBrowserAuthenticator(session),
			
			}); ;
			return await oidcClient.LoginAsync();
		}
	}


	
}
