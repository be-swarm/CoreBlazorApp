using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.CoreBlazorApp.Services
{
	public interface ICryptoService
	{
		Task<string> Encrypt(string plainText);
		Task<string> Decrypt(string cipherText);
	}
}
