using System;
using System.Collections.Generic;
using Server.Models;
using System.IO;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;

namespace Server.Cache.CacheUploader
{
    internal class JsonCacheUploader : ICacheUploader
    {
        public JsonCacheUploader(string fileName)
        {
            _fileName = fileName;
            string absolutePath = Environment.CurrentDirectory + "\\" + fileName;
            if (File.Exists(absolutePath))
            {
                //Read last cache
                string jsonContent = File.ReadAllText(absolutePath, System.Text.Encoding.UTF8);
                _fileStream = new FileStream(absolutePath, FileMode.Open);
                _fileStream.Seek(0, SeekOrigin.End);
                try
                {
                    //Parse json file
                    List<CurrencyInfo> currenciesInfo = JsonSerializer.Deserialize<List<CurrencyInfo>>(jsonContent);
                    currenciesInfo.ForEach(currencyInfo => _cacheCurrencies.Add(currencyInfo.ConvertToCurrencyRate()));
                }
                catch(Exception e)
                {
                    //If json was invalid
                    _fileStream.Close(); _fileStream.Dispose();
                    _fileStream = new FileStream(absolutePath, FileMode.Create);    //Create empty file
                    WriteStringToStream("[]");
                    _cacheCurrencies = new List<CurrencyRate>();
                }
            }
            else
            {
                _fileStream = new FileStream(absolutePath, FileMode.Create);
                WriteStringToStream("[]");
            }
        }


        //Get cache from the file/DB
        public IEnumerator<CurrencyRate> LoadCache()
        {
            return _cacheCurrencies.GetEnumerator();
        }


        //Append new rates to the file/DB
        public void AppendCurrencyRates(IEnumerator<CurrencyRate> rates)
        {
            CurrencyInfo currencyInfo = new CurrencyInfo();
            StringBuilder jsonContent = new StringBuilder();
            var options = new JsonSerializerOptions { WriteIndented = true };
            while (rates.MoveNext())
            {
                //Add comma, if there was some objects
                if (_cacheCurrencies.Count != 0)
                {
                    jsonContent.Append("," + Environment.NewLine);
                }
                //Serialize next rate
                currencyInfo.InitByCurrencyRate(rates.Current);
                jsonContent.Append(JsonSerializer.Serialize<CurrencyInfo>(currencyInfo, options));
                _cacheCurrencies.Add(rates.Current);
            }
            if (jsonContent.Length > 0)
            {
                _fileStream.Position -= 1;  //Rewrite last ']' symbol
                jsonContent.Append(Environment.NewLine + "]");
                WriteStringToStream(jsonContent.ToString());
            }
        }


        private void WriteStringToStream(string content)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
            _fileStream.Write(data, 0, data.Length);
            _fileStream.Flush();
        }


        //This struct is used to serialize into file
        private struct CurrencyInfo
        {
            public string Currency { get; set; }
            public string Date { get; set; }
            public double Value { get; set; }
            public byte Amount { get; set; }


            //Fill properties according to given currency rate
            public void InitByCurrencyRate(CurrencyRate currencyRate)
            {
                Currency = currencyRate.CurrencyTypeTo.ToString();
                Date = currencyRate.RateDate.ToString("dd/M/yy");
                Value = Math.Truncate(currencyRate.CurrencyAmountTo * 100) / 100;
                Amount = (byte)currencyRate.CurrencyCountFrom;
            }


            //Conver one model to another
            public CurrencyRate ConvertToCurrencyRate()
            {
                CurrencyRate rate = new CurrencyRate();
                rate.CurrencyTypeFrom = ConfigLibrary.Bean.CurrencyType.BYN;
                rate.CurrencyTypeTo = (ConfigLibrary.Bean.CurrencyType)Enum.Parse(typeof(ConfigLibrary.Bean.CurrencyType), Currency);
                rate.CurrencyCountFrom = Amount;
                rate.CurrencyAmountTo = Value;
                rate.RateDate = DateTime.Parse(Date);
                return rate;
            }

        }

        private string _fileName;   //Name of the cache file (in the directory of the server app)

        private List<CurrencyRate> _cacheCurrencies = new List<CurrencyRate>();

        private FileStream _fileStream;
    }
}
