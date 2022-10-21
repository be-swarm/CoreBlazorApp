using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blazored.SessionStorage;

using Newtonsoft.Json;

namespace BeSwarm.CoreBlazorApp.Services;


public class SessionPersistenceToSessionWeb : ISessionPersistence
{
	private ISessionStorageService SessionStorageService = default!;

	public SessionPersistenceToSessionWeb(ISessionStorageService _sessionloService)
	{
		SessionStorageService = _sessionloService;
	}
	public async Task Save(string key, string value)
	{
		await SessionStorageService.SetItemAsStringAsync(key, value);
	}

	public async Task<string> Get(string key)
	{
		return await SessionStorageService.GetItemAsStringAsync(key);

	}
}

