﻿using System;
using ConfigLibrary.Bean;

namespace Server.Models
{
    //Represents currency rate
    internal class CurrencyRate
    {

        public CurrencyType CurrencyTypeFrom { get; set; } //From which type of currency make rate (BYN, BTC)

        public CurrencyType CurrencyTypeTo { get; set; } //To which type of currency make rate (RUB, USD, ...)

        public int CurrencyCountFrom { get; set; }  //

        public int CurrencyAmountTo { get; set; }  //

        public DateTime RateDate { get; set; }

        //* All currencies, that have coins (BYN, RUB, USD) have rate x100 *

    }
}
