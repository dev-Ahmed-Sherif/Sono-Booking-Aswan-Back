using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace SonoBooking.Common.Helpers.HttpClient.RestSharp
{
    public class RestSharpClient(IHttpContextAccessor httpContextAccessor, ILogger<RestSharpClient> logger)
        : IRestSharpClient
    {
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        };
        public async Task<T> SendRequest<T>(string url, Method method, object obj = null, string urlEncoded = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest(url, method);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
                if (method == Method.Post || method == Method.Put)
                {
                    if (urlEncoded != null)
                    {
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddParameter("application/x-www-form-urlencoded", urlEncoded, ParameterType.RequestBody);
                    }
                    else
                    {
                        SetJsonContent(request, obj);
                    }
                }
                logger.LogInformation($"Rest-Sharp: Url  {url}");
                if (httpContextAccessor.HttpContext != null)
                {
                    var accessToken = await httpContextAccessor?.HttpContext?.GetTokenAsync("access_token")!;
                    if (accessToken != null) request.AddHeader("Authorization", "Bearer " + accessToken);
                }

                var response = await client.ExecuteAsync<T>(request);
                T data;
                try
                {
                    data = JsonConvert.DeserializeObject<T>(response.Content!);
                }
                catch (Exception e)
                {
                    data = default(T);
                    logger.LogInformation($"Rest-Sharp: Response Status Code{JsonConvert.SerializeObject(response.StatusCode)}");
                    logger.LogInformation($"Rest-Sharp: Error At Serializing Data {JsonConvert.SerializeObject(e, _serializerSettings)}");
                }
                return data == null ? response.Data : data;
            }
            catch (Exception e)
            {
                logger.LogError("Error At Rest Sharp Container" + JsonConvert.SerializeObject(e));
                throw;
            }
            
        }



        public async Task<T> SendBasicRequest<T>(string url, Method method, string username, string password, object obj = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var options = new RestClientOptions
                {
                    Authenticator = new HttpBasicAuthenticator(username, password)
                };
                var client = new RestClient(options);
                var request = new RestRequest(url, method);

                request.AddHeader("Accept", "application/json");
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }

                if (method == Method.Post || method == Method.Put)
                {
                    SetJsonContent(request, obj);
                }
                logger.LogInformation($"Rest-Sharp: URL {url}");
                var response = await client.ExecuteAsync<T>(request);
                T data;
                try
                {
                    data = JsonConvert.DeserializeObject<T>(response.Content!);

                }
                catch (Exception e)
                {
                    data = default(T);
                    logger.LogInformation($"Rest-Sharp: Response Status Code{JsonConvert.SerializeObject(response.StatusCode)}");
                    logger.LogInformation($"Rest-Sharp: Error At Serializing Data {JsonConvert.SerializeObject(e, _serializerSettings)}");
                }
                return data == null ? response.Data : data;
            }
            catch (Exception e)
            {
                logger.LogError("Error At Rest Sharp Client" + JsonConvert.SerializeObject(e.Message));
                throw;
            }

        }



        private void SetJsonContent(RestRequest request, object obj)
        {
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(obj);
        }


    }
}

