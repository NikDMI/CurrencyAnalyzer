using System;
using System.Net;
using ConfigLibrary.Bean;
using System.Net.Http;
using System.Collections.Generic;
using Client.Model;

namespace Client.Clients
{
    internal class HttpClient : IClient
    {
        public HttpClient(string serverDomain, int serverPort)
        {
            _serverUri = "http://" + serverDomain + ":" + serverPort + "/";
            _httpClient = new System.Net.Http.HttpClient();
        }


        //Send request to get currency rates. Updates data througth viewModel object (sync method)
        public List<Rate> SendRequestToNewRates(ViewModel viewModel, DateTime timeFrom, DateTime timeTo, CurrencyType currencyFrom,
            CurrencyType currencyTo)
        {
            List<Rate> responseRates = new List<Rate>();
            var requestData = NetworkPacketBuilder.CreateRequestPacket(currencyFrom, currencyTo, timeFrom, timeTo);
            HttpContent httpContent = new ByteArrayContent(requestData.ToArray());
            try
            {
                var responceTask = _httpClient.PostAsync(_serverUri, httpContent);
                responceTask.Wait();
                HttpResponseMessage serverResponse = responceTask.Result;
                var taskResponse = serverResponse.Content.ReadAsByteArrayAsync();
                taskResponse.Wait();
                List<byte> responseData = new List<byte>(taskResponse.Result);
                //Parse packet
                var ratesCurrencies = NetworkPacketBuilder.ParseResponseRatesPacket(responseData);
                foreach(var rateCurrency in ratesCurrencies)
                {
                    Rate rate = new Rate();
                    rate.Date = rateCurrency.Date;
                    rate.Value = rateCurrency.Amount;
                    responseRates.Add(rate);
                }
            } catch (Exception e)
            {
                //return responseRates;   //return empty list
            }
            return responseRates;
        }

        private string _serverUri;

        private System.Net.Http.HttpClient _httpClient;
    }
}
