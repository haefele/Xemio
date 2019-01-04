using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xemio.Client.Data.Endpoints.Auth;

namespace Xemio.Client
{
    public class XemioClient
    {
        private readonly HttpClient _client;

        public XemioClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback;

            this._client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
        }

        private bool ServerCertificateCustomValidationCallback(HttpRequestMessage arg1, X509Certificate2 arg2, X509Chain arg3, SslPolicyErrors arg4)
        {
            return true;
        }

        public async Task Register(RegisterAction action)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Register");
                request.Content = new StringContent(JsonConvert.SerializeObject(action), Encoding.UTF8, "application/json");

                await this.SendRequest(request, HttpStatusCode.OK);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when registering a new user.", e);
            }
        }

        public async Task<LoginResult> Login(LoginAction action)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Login");
                request.Content = new StringContent(JsonConvert.SerializeObject(action), Encoding.UTF8, "application/json");

                var response = await this.SendRequest(request, HttpStatusCode.OK);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<LoginResult>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when logging in.", e);
            }
        }
        
        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, HttpStatusCode expectedStatusCode)
        {
            var response = await this._client.SendAsync(request);

            if (response.StatusCode != expectedStatusCode)
            {
                var problemDetailsJson = await response.Content.ReadAsStringAsync();
                var problemDetails = JObject.Parse(problemDetailsJson);

                string title = problemDetails.Value<string>("title");
                string details = problemDetails.Value<string>("details");

                throw new XemioClientException(title + Environment.NewLine + details);
            }

            return response;
        }
    }

    public class XemioClientException : Exception
    {
        public XemioClientException(string message) 
            : base(message)
        {
        }

        public XemioClientException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}