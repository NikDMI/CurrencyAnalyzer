using System;
using ConfigLibrary.Bean;

namespace Server.Services
{
    //Factory to produce API object
    internal class CurrencyServiceFactory
    {
        //Returns special service for corresponding currenct
        public static ICurrencyService GetCurrencyService(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.BYN:
                    return new NbrbService();

                default:
                    throw new ArgumentException("There is no service API to work with " + currencyType.ToString());
            }
        }

        private CurrencyServiceFactory() { }
    }
}
