using BeSwarm.CoreBlazorApp.Services;
using Blazored.SessionStorage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMauiBlazor
{
	public class SessionPersistenceMaui : ISessionPersistence
	{
		
		public SessionPersistenceMaui()
		{
			
		}
		public async Task Save(string key, string value)
		{
			
		}

		public async Task<string> Get(string key)
		{
			return "";

		}
	}


}
