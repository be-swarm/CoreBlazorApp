using BeSwarm.WebApi.Access;
using BeSwarm.WebApi.Core;
using BeSwarm.WebApi.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;

using System.Security.Cryptography;
using System.Text;
using BeSwarm.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BeSwarm.WebApi;

public interface ISecureConfig
{
	string ServiceEntryPoint { get; }
	string UserSwarm { get; }
	string ApplicationId { get; }
	string ClientSecret { get; }
	string CallBackUri { get; }

}
internal class CurrentSession
{
	public TokenResult Usertoken { get; set; } = new();
	public TokenResult Applicationtoken { get; set; } = new();
	public string StateCode { get; set; } = "";
	public string CodeChallenge { get; set; } = "";


}
public enum SessionWebApiEvents
{
	Logout = 1,
	Login = 2,
	TokenRefreshed = 3,
	TraceHttpAdded = 4
}

public class SessionWebApi
{
	private CurrentSession _current = new();

	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ITraceHttp _traceHttp;
	private readonly ISecureConfig _secureConfig;
	public SessionWebApi(ISecureConfig secureConfig, IHttpClientFactory httpClientFactory, ITraceHttp traceHttp)
	{
		GetLangs = new List<ReferentialItem>();
		_httpClientFactory = httpClientFactory;
		_traceHttp = traceHttp;
		_secureConfig=secureConfig;
	
	}
	public string Secret => _secureConfig.ClientSecret;
	public ICollection<ReferentialItem> GetLangs { get; }
	public event Action<SessionWebApiEvents> SessionHasChanged = default!;

	public string UserToken => _current.Usertoken.Token_type + " " + _current.Usertoken.Id_token;
	public string UserTokenType => _current.Usertoken.Token_type;
	public string UserTokenValue => _current.Usertoken.Id_token;
	public ITraceHttp TracesHttp => _traceHttp;
	public string ApplicationToken => _current.Applicationtoken.Token_type + " " + _current.Applicationtoken.Id_token;

	public bool SessionIsActive
	{
		get
		{
			if (_current.Usertoken is { } && _current.Applicationtoken is { } &&
				!string.IsNullOrEmpty(_current.Usertoken.Id_token) &&
				!string.IsNullOrEmpty(_current.Applicationtoken.Id_token)) return true;
			return false;
		}
	}

	public string SerializeCurrentSession()
	{
		return JsonConvert.SerializeObject(_current);
	}

	public void DeserializeCurrentSession(string session)
	{
		CurrentSession res = JsonConvert.DeserializeObject<CurrentSession>(session);
		_current = res is { } ? res : new CurrentSession();
	}

	//
	// return oauth login url
	// generate new statecode and codechallenge
	//
	public string GetLoginUrl(string callbackuri,string user = null, string password = null, string secretkey = null,
		string lang = "FR")
	{
		_current.StateCode = Guid.NewGuid().ToString();

		_current.CodeChallenge = Guid.NewGuid().ToString();
		string url =
			$"{_secureConfig.ServiceEntryPoint}/authorize?&state={_current.StateCode}&appid={_secureConfig.ApplicationId}&lang={lang}&callbackuri={callbackuri}";

		string hashValue = "";

		using (SHA256 sha256Hash = SHA256.Create())
		{
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(_current.CodeChallenge));
			hashValue = Convert.ToHexString(bytes);
		}

		url = url + $"&code_challenge={hashValue}";
		url = url + $"&swarm={_secureConfig.UserSwarm}";
		if (user != null) url = url + $"&login={user}";
		if (password != null) url = url + $"&password={password}";
		if (secretkey != null) url = url + $"&cryptokey={secretkey}";
		string res = url;
		return res;
	}

	// 
	// get users token from callbackurl
	//
	public async Task<ResultAction> GetUserTokens(string url)
	{
		ResultAction ret = new();
		string code, swarm, serviceurl;
		string state = code = swarm = serviceurl = "";
		Uri uri = new(url);
		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out Microsoft.Extensions.Primitives.StringValues scode)) code = scode;
		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("state", out Microsoft.Extensions.Primitives.StringValues sstate)) state = sstate;
		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("swarm", out Microsoft.Extensions.Primitives.StringValues sswarm)) swarm = sswarm;
		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("serviceurl", out Microsoft.Extensions.Primitives.StringValues sserviceurl)) serviceurl = sserviceurl;
		if (code != "" && swarm != "" && state != "" && serviceurl != "")
		{
			Dictionary<string, string> keys = new();
			keys["code"] = code;
			keys["state"] = state;
			keys["swarm"] = swarm;
			keys["serviceurl"] = serviceurl;
			return await GetUserTokens(keys);
		}

		ret.SetError(new InternalError { Description = "invalid url or missing parameters" },
			StatusAction.Logicalerror);
		return ret;
	}


	//
	// get user tokens from authorization code
	//
	public async Task<ResultAction> GetUserTokens(Dictionary<string, string> keys)
	{
		ResultAction ret = new();
		// is for my login call ?
		if (keys["state"] != _current.StateCode)
		{
			ret.SetError(new InternalError { Description = "invalid state_code" }, StatusAction.Unauthorized);
			return ret;
		}

		_current.Usertoken = _current.Applicationtoken = null;
		var httpclient = _httpClientFactory.CreateClient("BeSwarm");
		httpclient.BaseAddress = new Uri(keys["serviceurl"]);
		Access.Access authorize = new("", new BeSwarmHttpClient(httpclient, this));
		GetTokenFromAuthorisationCode get = new();
		get.Client_secret = _secureConfig.ClientSecret;
		get.Code = keys["code"];
		get.Code_challenge = _current.CodeChallenge;
		get.Swarm = keys["swarm"];
		try
		{
			AuthResultResultAction res = await authorize.GetTokensFromAuthorizationCodeAsync(get);
			_current.Applicationtoken = res.Datas.Applicationtoken;
			_current.Usertoken = res.Datas.Usertoken;
			SessionHasChanged?.Invoke(SessionWebApiEvents.Login);
		}
		catch (Exception e)
		{
			ret = GetInternalErrorFromException(e);
		}
		return ret;
	}
	public async Task<ResultAction> RefreshUserToken()
	{
		ResultAction res = new();
		var httpclient = GetUserHttpClient();
		Access.Access authorize = new("", httpclient);
		GetRefreshToken getrefresh = new()
		{
			Token = UserToken,
			Code_challenge = _current.CodeChallenge,
		};
		try
		{
			var resrefresh = await authorize.GetRefreshedTokenAsync(getrefresh);
			_current.Usertoken = resrefresh.Datas;
			// Inform usertoken has changed
			SessionHasChanged?.Invoke(SessionWebApiEvents.TokenRefreshed);
		}
		catch (Exception e)
		{
			var reserror = GetInternalErrorFromException(e);
			res.SetError(reserror.Error, reserror.Status);
		}
		return res;
	}

	public void LogOut()
	{
		_current.Usertoken = _current.Applicationtoken = new TokenResult();
		SessionHasChanged?.Invoke(SessionWebApiEvents.Logout);
	}

	public async Task AddTraceHttp(Trace add)
	{
		await _traceHttp.AddTrace(add);
		SessionHasChanged?.Invoke(SessionWebApiEvents.TraceHttpAdded);
	}
	public BeSwarmHttpClient GetUserHttpClient()
	{

		var httpclient = _httpClientFactory.CreateClient("BeSwarm");

		string ep = "";
		if (_current.Usertoken.Hosts is { } && _current.Usertoken.Hosts.Count >= 1) ep = _current.Usertoken.Hosts.First();
		else ep = _secureConfig.ServiceEntryPoint;
		httpclient.BaseAddress = new Uri(ep);
		return new(httpclient, this);
	}

	public async Task<ResultAction<List<ReferentialItemLang>>> GetReferentialItems(string referential, string lang)
	{
		ResultAction<List<ReferentialItemLang>> res = new();
		if (!SessionIsActive)
		{
			res.SetError(new InternalError { Description = "User is not logged" }, StatusAction.Logicalerror);
			return res;
		}

		var httpclient = GetUserHttpClient();
		Referentials.Referentials refs = new("", httpclient);
		try
		{
			Referentials.ReferentialItemListResultAction result = await refs.GetReferentialListAsync(UserToken, referential);
			foreach (ReferentialItem item in result.Datas)
			{
				ReferentialItemLang add = new ReferentialItemLang
				{
					Uidreferential = item.Uidreferential,
					Uid = item.Uid,
					Codelang = lang,
					Description = item.GetDescriptionInLang(lang),
					Thumbnail = item.Thumbnail
				};
				res.Datas.Add(add);
			}
		}
		catch (Exception e)
		{
			// show error if needed
			ResultAction err = GetInternalErrorFromException(e);
			res.CopyStatusFrom(err);
		}

		return res;
	}

	public ResultAction GetInternalErrorFromException(Exception e)
	{
		ResultAction ret = new();
		if (e is BeSwarmWebApiException<ProblemDetails>)
		{
			BeSwarmWebApiException<ProblemDetails> st;
			st = e as BeSwarmWebApiException<ProblemDetails>;
			int code = st.StatusCode;

			IDictionary<string, object> d = st.Result.AdditionalProperties;
			foreach (KeyValuePair<string, object> item in d)
			{
				if (item.Key != "error") continue;
				ret.Error = JsonConvert.DeserializeObject<InternalError>(item.Value.ToString());
				ret.Status = code;
				break;
			}
		}
		else
		{
			ret.SetError(new InternalError { Description = e.Message }, 500);
			ret.Status = 500;
		}

		return ret;
	}
}