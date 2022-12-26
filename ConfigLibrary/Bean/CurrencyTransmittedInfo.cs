using System;
using System.Collections.Generic;

namespace ConfigLibrary.Bean
{
    //Object of this class can be transmitted throught the network
    public struct CurrencyTransmittedInfo : IConvertable
    {
        public DateTime Date { get; set; }  //date of the rate

        public int Value { get; set; }  //value in x100 BYN (257 = 2.57 BYN)

        public int Amount { get; set; } // value of foreign currency


        //Get serialized bytes of some structure
        public List<byte> SerializeData()
        {
            return BinaryConverter.SerializeData<CurrencyTransmittedInfo>(this);
        }


        //Fill structure from serialized bytes
        public void DeserializeData(List<byte> data)
        {
            BinaryConverter.DeserializeData<CurrencyTransmittedInfo>(data, ref this);
        }
    }
}
