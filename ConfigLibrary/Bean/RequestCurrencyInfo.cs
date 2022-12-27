using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    //Identify currencies that user wants to get
    public class RequestCurrencyInfo : IConvertable
    {
        public int CurrencyTypeFrom { get; set; }
        public int CurrencyTypeTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }


        //Get serialized bytes of some structure
        public List<byte> SerializeData()
        {
            return BinaryConverter.SerializeData(this);
        }


        //Fill structure from serialized bytes
        public void DeserializeData(List<byte> data)
        {
            BinaryConverter.DeserializeData<RequestCurrencyInfo>(data, this);
        }
    }
}
