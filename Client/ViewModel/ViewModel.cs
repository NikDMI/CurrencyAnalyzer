using System;
using System.Collections.Generic;
using ConfigLibrary;
using Client.Clients;
using Client.Model;
using ConfigLibrary.Bean;

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
            _currencyRates = _client.SendRequestToNewRates(this, dateFrom, dateTo, currencyFrom, currencyTo);
            _viewWindow.UpdateGraphicsAsync(_currencyRates);
        }

        //public CurrencyType CurrencyTypeTo { get { return _currencyTypeTo; } set { _currencyTypeTo = value; } }


        private IClient _client;

        private MainWindow _viewWindow;

        private List<Rate> _currencyRates;  //current Rates

        //private CurrencyType _currencyTypeTo;   //To which currency we make graphics
    }
}
