using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xemio.Client.Data.Endpoints.Auth;
using Xemio.Client.Data.Endpoints.Notebooks;
using Xemio.Client.Data.Entities;

namespace Xemio.Client
{
    public class XemioClient
    {
        private readonly HttpClient _client;

        public XemioClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = this.ServerCertificateCustomValidationCallback;

            this._client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
        }

        #region Auth
        public async Task Register(RegisterAction action)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Register")
                {
                    Content = new JsonStringContent(action)
                };

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
                var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Login")
                {
                    Content = new JsonStringContent(action)
                };

                var response = await this.SendRequest(request, HttpStatusCode.OK);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<LoginResult>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when logging in.", e);
            }
        }
        #endregion

        #region Notebooks
        public async Task<NotebookDTO> CreateNotebook(CreateNotebookAction action, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/Notebooks")
                {
                    Content = new JsonStringContent(action)
                };

                var response = await this.SendRequest(request, HttpStatusCode.Created, token);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<NotebookDTO>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when creating the notebook.", e);
            }
        }

        public async Task<NotebookDTO> GetNotebook(string notebookId, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/Notebooks/{notebookId}");

                var response = await this.SendRequest(request, HttpStatusCode.OK, token);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<NotebookDTO>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when loading the notebook.", e);
            }
        }

        public async Task<NotebookHierarchyDTO> GetNotebookHierarchy(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Notebooks/Hierarchy");

                var response = await this.SendRequest(request, HttpStatusCode.OK, token);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<NotebookHierarchyDTO>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when loading the notebook hierarchy.", e);
            }
        }

        public async Task<NotebookDTO> UpdateNotebook(string notebookId, UpdateNotebookAction action, string token)
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/Notebooks/{notebookId}")
                {
                    Content = new JsonStringContent(action)
                };

                var response = await this.SendRequest(request, HttpStatusCode.OK, token);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<NotebookDTO>(content);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when updating the notebook.", e);
            }
        }

        public async Task DeleteNotebook(string notebookId, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"/Notebooks/{notebookId}");
                await this.SendRequest(request, HttpStatusCode.OK, token);
            }
            catch (Exception e) when (e is XemioClientException == false)
            {
                throw new XemioClientException("An error occurred when deleting the notebook.", e);
            }
        }
        #endregion

        #region Internal
        private bool ServerCertificateCustomValidationCallback(HttpRequestMessage arg1, X509Certificate2 arg2, X509Chain arg3, SslPolicyErrors arg4)
        {
            return true;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, HttpStatusCode expectedStatusCode, string token = null)
        {
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

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

        private class JsonStringContent : StringContent
        {
            public JsonStringContent(object content)
                : base(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            {
                
            }
        }
        #endregion
    }
}