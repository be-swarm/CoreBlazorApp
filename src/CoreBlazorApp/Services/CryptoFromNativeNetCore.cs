using BeSwarm.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.CoreBlazorApp.Services
{
	public class CryptoFromNativeNetCore : ICryptoService
	{
		private byte[] key;
		private byte[] iv;
		public CryptoFromNativeNetCore(ISecureConfig config)
		{
			// prepare AES encryption
			key=Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(config.ClientSecret), Encoding.UTF8.GetBytes(config.ApplicationId), iterations: 10000, HashAlgorithmName.SHA256,outputLength: 32);
			iv = key[0..16];
		}


		//
		// encrypt a string
		// return base64 encoded string
		//
		public async Task<string> Encrypt(string plainText)
		{
			string result = "";
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Mode = CipherMode.CBC;
				aesAlg.Key = key;
				aesAlg.IV = iv;
				// Create a decryptor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
				
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}

						var encrypted = msEncrypt.ToArray();
						result = Convert.ToBase64String(encrypted);
					}
				}
			}
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
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Mode = CipherMode.CBC;
				aesAlg.Key = key;
				aesAlg.IV = iv;
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				try
				{

					using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(base64cipherText)))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{
								result = srDecrypt.ReadToEnd();

							}
						}
					}
				}
				catch (Exception e)
				{
				}

			}

			return result;
		}
		
	}
	
}
