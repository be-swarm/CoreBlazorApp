using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.CoreBlazorApp.Services;


	public  interface ISessionPersistence
	{

		Task Save(string key, string value);
		Task<string> Get(string key);
	}

