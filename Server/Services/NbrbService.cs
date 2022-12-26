using System;
using System.Collections.Generic;
using Server.Models;
using ConfigLibrary.Bean;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Server.Services.Models;
using System.Globalization;

namespace Server.Services
{
    //This API gives rates according to BYN
    internal class NbrbService : ICurrencyService
    {
        public NbrbService()
        {
            //Require all available currencies
            var requestTask = _httpClient.GetStringAsync("https://www.nbrb.by/api/exrates/currencies");
            requestTask.Wait();
            string availableCurrenciesJson = requestTask.Result;
            List<CurrencyNbrb> currenciesInfo = JsonSerializer.Deserialize<List<CurrencyNbrb>>(availableCurrenciesJson);
            //Register currencies info
            _currenciesInfo = new Dictionary<CurrencyType, CurrencyNbrb>();
            foreach (var currencyName in Enum.GetNames(typeof(CurrencyType)))
            {
                var assosiatedCurrencyInfo = 
                    currenciesInfo.Find(currencyNbrb => currencyNbrb.Cur_Abbreviation == currencyName ? true : false);
                if (assosiatedCurrencyInfo != null)
                {
                    _currenciesInfo.Add((CurrencyType)Enum.Parse(typeof(CurrencyType), currencyName), assosiatedCurrencyInfo);
                }
            }
        }


        //Returns list of rates in the period [timeFrom; timeTo]
        public List<CurrencyRate> GetRates(DateTime timeFrom, DateTime timeTo, CurrencyType currencyType)
        {
            if (timeFrom > timeTo)
            {
                throw new ArgumentException("Bad time dimensions");
            }
            CurrencyNbrb currencyNbrb = null;
            if (!_currenciesInfo.TryGetValue(currencyType, out currencyNbrb))
            {
                throw new ArgumentException("Nbrb not supported " + currencyType.ToString() + " currency");
            }
            //Requere rates
            var rateTimespan = timeTo - timeFrom;   //How mush days user want get
            while (rateTimespan > TimeSpan.Zero)
            {
                if (rateTimespan > AVAILABLE_DAYS_PERIOD)
                {
                    timeTo = timeFrom + AVAILABLE_DAYS_PERIOD;
                    rateTimespan -= AVAILABLE_DAYS_PERIOD;
                }
                else
                {
                    timeTo = timeFrom + rateTimespan;
                    rateTimespan = TimeSpan.Zero;
                }
                GetDynamicRates(timeFrom, timeTo, currencyNbrb);
                timeFrom = timeTo;
            }
            return null;
        }


        private List<RateShort> GetDynamicRates(DateTime fromDay, DateTime toDay, CurrencyNbrb currency)
        {
            //Require all available currencies
            string requestString = "https://www.nbrb.by/API/ExRates/Rates/Dynamics/" + currency.Cur_ID + "?startDate=" +
                fromDay.ToString("yyyy-M-d") + "&endDate=" + toDay.ToString("yyyy-M-d");
            var requestTask = _httpClient.GetStringAsync(requestString);
            requestTask.Wait();
            string responseRates = requestTask.Result;
            List<RateShort> rates = JsonSerializer.Deserialize<List<RateShort>>(responseRates);
            if (rates.Count == 0)   //If api service give invalid results
            {
                TimeSpan dynamicPeriod = toDay - fromDay + TimeSpan.FromDays(1); 
                //This brude cycle is used, when first request returns empty array
                while (dynamicPeriod > TimeSpan.Zero)
                {
                    requestString = "https://www.nbrb.by/api/exrates/rates/" + currency.Cur_Abbreviation + "?parammode=2&ondate=" +
                        fromDay.ToString("yyyy-M-d");
                    requestTask = _httpClient.GetStringAsync(requestString);
                    requestTask.Wait();
                    string responseRate = requestTask.Result;
                    Rate rate = JsonSerializer.Deserialize<Rate>(responseRate);
                    rates.Add(new RateShort { Cur_ID = rate.Cur_ID, Cur_OfficialRate = rate.Cur_OfficialRate, Date = rate.Date });
                    fromDay += TimeSpan.FromDays(1);
                    dynamicPeriod -= TimeSpan.FromDays(1);
                }
            }
            return rates;
        }


        private TimeSpan AVAILABLE_DAYS_PERIOD = TimeSpan.FromDays(365);   //How much days are available to get rate from one request

        private HttpClient _httpClient = new HttpClient();

        private Dictionary<CurrencyType, CurrencyNbrb> _currenciesInfo; //Assosiated info with required currency
    }
}
