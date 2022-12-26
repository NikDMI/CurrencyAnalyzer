using System;
using System.Net;

namespace Client.Clients
{
    internal class HttpClient : IClient
    {
        public HttpClient(string serverDomain, int serverPort)
        {
            _serverUri = "http://" + serverDomain + ":" + serverPort + "/";
            _httpClient = new System.Net.Http.HttpClient();
        }


        public void SendTest()
        {
            //var task = _httpClient.GetByteArrayAsync(_serverUri);
            //task.Wait();
            //var res = task.Result;
        }

        private string _serverUri;

        private System.Net.Http.HttpClient _httpClient;
    }
}
