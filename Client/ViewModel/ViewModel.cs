using System;
using System.Collections.Generic;
using ConfigLibrary;
using Client.Clients;
using Client.Model;
using ConfigLibrary.Bean;
using System.Threading.Tasks;

namespace Client
{
    internal class ViewModel
    {
        public ViewModel(IConfig appConfigs, MainWindow viewWindow)
        {
            _client = ClientsFactory.GetClientFromAppConfig(appConfigs);
            _viewWindow = viewWindow;
        }


        //Sends requests and updates graphics
        public void UpdateRates(DateTime dateFrom, DateTime dateTo, CurrencyType currencyFrom, CurrencyType currencyTo)
        {
            Task.Run(()=>
            {
                _viewWindow.StartProcessRequest();
                _currencyRates.Clear();
                List<Rate> currentRates = new List<Rate>();
                TimeSpan requestDays = dateTo - dateFrom + TimeSpan.FromDays(1);
                DateTime startDate = new DateTime(dateFrom.ToBinary());
                while(requestDays > TimeSpan.Zero)
                {
                    TimeSpan currentRequestTimeSpan = (requestDays > MAX_DAYS_IN_ONE_REQUEST - TimeSpan.FromDays(1)) ?
                    (MAX_DAYS_IN_ONE_REQUEST - TimeSpan.FromDays(1)) : requestDays;
                    //Requare next days
                    currentRates.AddRange(_client.SendRequestToNewRates(this, startDate, startDate + currentRequestTimeSpan,
                        currencyFrom, currencyTo));
                    _viewWindow.UpdateGraphicsAsync(currentRates);//Update function (make method o add new rates)
                    startDate += currentRequestTimeSpan;
                    requestDays -= currentRequestTimeSpan;
                }
                _currencyRates = currentRates;
                _viewWindow.StopProcessRequest();
            });
        }

        //public CurrencyType CurrencyTypeTo { get { return _currencyTypeTo; } set { _currencyTypeTo = value; } }


        private IClient _client;

        private MainWindow _viewWindow;

        private List<Rate> _currencyRates = new List<Rate>();  //current Rates

        private static readonly TimeSpan MAX_DAYS_IN_ONE_REQUEST = TimeSpan.FromDays(10);   //Indicate how mush days can be performed by one request

    }
}
