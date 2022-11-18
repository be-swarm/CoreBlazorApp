using BeSwarm.WebApi.Models;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BeSwarm.WebApi.Services;
using System.Net;

namespace BeSwarm.WebApi
{
    public class BeSwarmHttpClient:IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SessionWebApi _sessionWebApi;
      
        public BeSwarmHttpClient(HttpClient httpClient, SessionWebApi sessionWebApi)
        {
            _httpClient = httpClient;
            _sessionWebApi = sessionWebApi;
        }

        public void SetBaseAddress(string url)
        {
            _httpClient.BaseAddress = new Uri(url);
        }
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            HttpRequestMessage tosend = request;
            HttpResponseMessage res = null;
           
            for (int i = 1; i <= 2; i++)   // two times one for initial and second for retry if token is expired
            {
                res = await _httpClient.SendAsync(tosend, completionOption,cancellationToken);
                await _sessionWebApi.AddTraceHttp(new(DateTime.UtcNow, res.StatusCode, request.RequestUri, request.Method));
                var content = await res.Content.ReadAsStringAsync();
                try
                {
                    var resultaction = JsonConvert.DeserializeObject<ResultAction>(content);

                    if (resultaction is { } && resultaction.Status == StatusAction.Unauthorized &&
                        resultaction.Error.ErrorCode == -1) // token is expired  or not refreshable ?
                    {
                        //
                        // todo: Check if token is an user token or application token
                        // todo: to call appropriate refresh token method
                        //
                        var refreshed = await _sessionWebApi.RefreshUserToken();
                        if (refreshed.IsOk)
                        {
                            
                            // new request
                            tosend = new HttpRequestMessage(request.Method, request.RequestUri);
                            tosend.Content = request.Content;
                            // change Authorization header
                            tosend.Headers.Authorization = new AuthenticationHeaderValue(_sessionWebApi.UserTokenType, _sessionWebApi.UserTokenValue);
                  
                        }
                        else
                        {
							// unable to get refreshed token
							_sessionWebApi.LogOut();
							break;
                        }
                    }
                    else if (resultaction is { } && resultaction.Status == StatusAction.Unauthorized)
                    {
	                    _sessionWebApi.LogOut();
	                    break;
					}
                    else break;

                }
                catch (Exception e)
				{
					await _sessionWebApi.AddTraceHttp(new(DateTime.UtcNow, res.StatusCode, request.RequestUri, request.Method, e.Message));
					break;
                }
            }
            return res;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
