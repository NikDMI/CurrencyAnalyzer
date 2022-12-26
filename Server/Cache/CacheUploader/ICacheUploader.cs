using System;
using System.Collections.Generic;
using Server.Models;

namespace Server.Cache
{
    //This interface represend uploaders to load/save cache
    internal interface ICacheUploader
    {
        //Get cache from the file/DB
        public IEnumerator<CurrencyRate> LoadCache();


        //Append new rates to the file/DB
        public void AppendCurrencyRates(IEnumerator<CurrencyRate> rates);
    }
}
