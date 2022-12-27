using System;
using System.Collections.Generic;
using System.Text;
using ConfigLibrary.Bean;
using Client.Model;

namespace Client.Clients
{
    //Interface of interaction between client and server
    internal interface IClient
    {
        //Send request to get currency rates. Updates data througth viewModel object
        public List<Rate> SendRequestToNewRates(ViewModel viewModel, DateTime timeFrom, DateTime timeTo, CurrencyType currencyFrom,
            CurrencyType currencyTo);
    }
}
