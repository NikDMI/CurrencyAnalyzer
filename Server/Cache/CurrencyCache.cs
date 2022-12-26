using System;
using System.Collections.Generic;
using ConfigLibrary.Bean;
using Server.Models;

namespace Server.Cache
{
    //Represents all currencies rates, that now available to the server
    internal class CurrencyCache
    {

        public CurrencyCache()
        {
            //Load from disk
            //Sort
        }


        //Outer dictionary: from which currency make rate (BYN, BTC in the task)
        //Internal dictionary: currency to which make rate -> list of rates by the special date (sorted by date)
        private Dictionary<CurrencyType, Dictionary<CurrencyType, List<CurrencyRate>>> _currenciesRates;
    }
}
