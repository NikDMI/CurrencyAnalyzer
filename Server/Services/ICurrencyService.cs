using System;
using Server.Models;
using System.Collections.Generic;
using ConfigLibrary.Bean;

namespace Server.Services
{
    //This interface is used to get rates from different API and different currencies
    internal interface ICurrencyService
    {
        //Returns list of rates in the period [timeFrom; timeTo]
        public List<CurrencyRate> GetRates(DateTime timeFrom, DateTime timeTo, CurrencyType currencyType);
    }
}
