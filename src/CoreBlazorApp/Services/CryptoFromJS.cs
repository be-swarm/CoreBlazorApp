using BeSwarm.WebApi;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.CoreBlazorApp.Services
{
	public class CryptoFromJS : ICryptoService
	{
		private ISecureConfig _config;
		private IJSRuntime _jsRuntime;
	
		public CryptoFromJS(ISecureConfig config, IJSRuntime JSRuntime)
		{
			_config = config;
			_jsRuntime = JSRuntime;

		}
		//
		// encrypt a string
		// return base64 encoded string
		//
		public async Task<string> Encrypt(string plainText)
		{
			string result = "";
			result = await _jsRuntime.InvokeAsync<string>("CoreBlazorApp.EncryptAES", _config.ClientSecret, _config.ApplicationId, plainText);
			return result;
		}

		//
		// decrypt a string
		// from a base64 encoded string
		// return plain text
		//
		public async Task<string> Decrypt(string base64cipherText)
		{
			string result = "";
			result = await _jsRuntime.InvokeAsync<string>("CoreBlazorApp.DecryptAES", _config.ClientSecret, _config.ApplicationId, base64cipherText);
			return result;
		}
	}

}
