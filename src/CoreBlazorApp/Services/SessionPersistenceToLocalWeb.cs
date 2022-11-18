using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blazored.LocalStorage;

using Newtonsoft.Json;

namespace BeSwarm.CoreBlazorApp.Services;


public class SessionPersistenceToLocalWeb : ISessionPersistence
{
	private readonly ILocalStorageService _localStorageService = default!;

	public SessionPersistenceToLocalWeb(ILocalStorageService _localService)
	{
		_localStorageService = _localService;
	}
	public async Task Save(string key, string value)
	{
		await _localStorageService.SetItemAsStringAsync(key, value);
	}

	public async Task<string> Get(string key)
	{
		return await _localStorageService.GetItemAsStringAsync(key);

	}
}

