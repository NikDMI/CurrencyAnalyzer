using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    //Help to crete array of bytes from packets
    public static class NetworkPacketBuilder
    {
        public static List<byte> CreateComplexPacket(params IConvertable[] packets)
        {
            List<byte> resultPacket = new List<byte>();
            foreach (var packet in packets)
            {
                resultPacket.AddRange(packet.SerializeData());
            }
            return resultPacket;
        }


        public static List<byte> CreateRequestPacket(CurrencyType currencyFrom, CurrencyType currencyTo, DateTime dateFrom, DateTime dateTo)
        {
            PacketInfo headerPacket = new PacketInfo(PacketInfo.PacketType.GET_RATES);
            RequestCurrencyInfo requestPacket = new RequestCurrencyInfo();
            requestPacket.CurrencyTypeFrom = (int)currencyFrom;
            requestPacket.CurrencyTypeTo = (int)currencyTo;
            requestPacket.DateFrom = dateFrom;
            requestPacket.DateTo = dateTo;
            return CreateComplexPacket(headerPacket, requestPacket);
        }


        //Create response from server
        public static List<byte> CreateResponseRatesPacket(PacketInfo header, ResponseRatesInfo responseInfo,
            IEnumerator<CurrencyTransmittedInfo> rates)
        {
            var responceList = CreateComplexPacket(header, responseInfo);
            while (rates.MoveNext())
            {
                responceList.AddRange(rates.Current.SerializeData());
            }
            return responceList;
        }


        //Parse response
        public static List<CurrencyTransmittedInfo> ParseResponseRatesPacket(List<byte> response)
        {
            PacketInfo headerPacket = new PacketInfo(PacketInfo.PacketType.ERROR);
            headerPacket.DeserializeData(response);
            if (headerPacket.Type != (int)PacketInfo.PacketType.RESPONSE_RATES)
            {
                throw new Exception("Invalid packet");
            }
            ResponseRatesInfo responseRatesInfo = new ResponseRatesInfo();
            responseRatesInfo.DeserializeData(response);
            List<CurrencyTransmittedInfo> rates = new List<CurrencyTransmittedInfo>(responseRatesInfo.RatesCount);
            for (int i = 0; i < responseRatesInfo.RatesCount; ++i)
            {
                CurrencyTransmittedInfo rate = new CurrencyTransmittedInfo();
                rate.DeserializeData(response);
                rates.Add(rate);
            }
            return rates;
        }
    }
}
