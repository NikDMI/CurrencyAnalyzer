using System;
using System.Collections.Generic;
using ConfigLibrary.Bean;
using Server.Models;
using Server.Services;
using System.Linq;
using System.Collections.Concurrent;

namespace Server.Cache
{
    //Represents all currencies rates, that now available to the server
    internal class CurrencyCache
    {

        public CurrencyCache()
        {
            //Initialize dictionaties
            _currenciesRates = new Dictionary<CurrencyType, Dictionary<CurrencyType, ConcurrentBag<CurrencyRate>>>();
            foreach(CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                var innerDictionary = new Dictionary<CurrencyType, ConcurrentBag<CurrencyRate>>();
                foreach (CurrencyType currencyInner in Enum.GetValues(typeof(CurrencyType)))
                {
                    if (currencyInner == currency) continue;
                    var currencyRates = new ConcurrentBag<CurrencyRate>();
                    innerDictionary.Add(currencyInner, currencyRates);
                }
                _currenciesRates.Add(currency, innerDictionary);
            }
            //Initialize services
            _currencyServices = new Dictionary<CurrencyType, ICurrencyService>();
            _currencyServices.Add(CurrencyType.BYN, CurrencyServiceFactory.GetCurrencyService(CurrencyType.BYN));
        }


        /// 
        /// Returns all available rates in time period
        /// All rates, that are not loaded, can be resolved by API calls
        /// 
        /// <param name="maxRequestCount">Restriction of API calls to outer resources</param>
        /// <returns>empty list if some errors occurers</returns>
        public List<CurrencyRate> GetAvailableCurrencyRates(CurrencyType currencyFrom, CurrencyType currencyTo,
            DateTime timeFrom, DateTime timeTo, int maxRequestCount)
        {
            ICurrencyService currencyService = null;
            if (!_currencyServices.TryGetValue(currencyFrom, out currencyService))
            {
                throw new ArgumentException("Can't get rates for this currency");
            }
            ConcurrentBag<CurrencyRate> currencyRates = _currenciesRates[currencyFrom][currencyTo];   //Rates in cache
            List<CurrencyRate> requeredRates = new List<CurrencyRate>();
            //Get all available rates in cache (make more optimaze algorithm at the state of refactoring)
            var availableRates = from rate in currencyRates
                                 where (rate.RateDate >= timeFrom && rate.RateDate <= timeTo)
                                 orderby rate.RateDate
                                 select rate;
            requeredRates.AddRange(availableRates);
            //Get not saved rates throught the API services
            RequareNotLoadedRates(requeredRates, currencyRates, currencyTo, timeFrom, timeTo, maxRequestCount, currencyService);
            return requeredRates;
            
        }


        //Requare up to maxRequestCount new records from services 
        private void RequareNotLoadedRates(List<CurrencyRate> requeredRates, ConcurrentBag<CurrencyRate> currencyRates, CurrencyType currencyTo,
            DateTime timeFrom, DateTime timeTo, int maxRequestCount, ICurrencyService currencyService)
        {
            int requestRatesCount = (timeTo - timeFrom).Days + 1;
            if (requestRatesCount > requeredRates.Count)
            {
                List<CurrencyRate> newCurrencyRates = new List<CurrencyRate>();
                DateTime startTime = timeFrom;
                IEnumerator<CurrencyRate> loadedRates = requeredRates.GetEnumerator();
                int neededRecords = requestRatesCount - requeredRates.Count;    //Computes records count
                neededRecords = neededRecords > maxRequestCount ? maxRequestCount : neededRecords;
                //Read new rates
                while (neededRecords > 0)
                {
                    int savedRecords = 0;   //New records, that we get from API
                    if (loadedRates.MoveNext()) //Check interval between loaded and unloaded rates
                    {
                        TimeSpan dynamicPeriod = loadedRates.Current.RateDate - startTime;
                        if (dynamicPeriod > TimeSpan.Zero)
                        {
                            savedRecords = dynamicPeriod.Days < neededRecords ? dynamicPeriod.Days : neededRecords;
                        }
                        else
                        {
                            startTime += TimeSpan.FromDays(1);
                        }
                    }
                    else  //Read max needed rates
                    {
                        savedRecords = neededRecords;
                    }
                    //Get new rates
                    if (savedRecords > 0)
                    {
                        newCurrencyRates.AddRange(currencyService.GetRates(startTime, startTime + TimeSpan.FromDays(savedRecords - 1), currencyTo));
                        startTime += TimeSpan.FromDays(savedRecords);
                        neededRecords -= savedRecords;
                    }
                }
                requeredRates.AddRange(newCurrencyRates);
                newCurrencyRates.ForEach(rate => currencyRates.Add(rate));//переделать с учетом вариата, когда два потока добавляют одни и те же записи
            }
        }

        //Outer dictionary: from which currency make rate (BYN, BTC in the task)
        //Internal dictionary: currency to which make rate -> list of rates by the special date (sorted by date)
        private Dictionary<CurrencyType, Dictionary<CurrencyType, ConcurrentBag<CurrencyRate>>> _currenciesRates;

        //Map of services to get rates of assosiated currency
        private Dictionary<CurrencyType, ICurrencyService> _currencyServices;

    }
}
