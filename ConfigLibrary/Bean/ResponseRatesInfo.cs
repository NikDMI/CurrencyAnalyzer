using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    public class ResponseRatesInfo : IConvertable
    {
        public int RatesCount { get; set; } //Represents number of next CurrencyTransmittedInfo from the server


        //Get serialized bytes of some structure
        public List<byte> SerializeData()
        {
            return BinaryConverter.SerializeData(this);
        }


        //Fill structure from serialized bytes
        public void DeserializeData(List<byte> data)
        {
            BinaryConverter.DeserializeData(data, this);
        }
    }
}
