using System;
using System.Collections.Generic;
using ConfigLibrary.Bean;
using Server.Cache;

namespace Server.RequestController
{
    //controlls user requests
    internal static class RequestController
    {
        //Parse user request and create response if needed
        public static List<byte> ProcessRequest(byte[] userRequest)
        {
            List<byte> binaryUserData = new List<byte>(userRequest);
            //Get packet info
            PacketInfo packetInfo = new PacketInfo(PacketInfo.PacketType.ERROR);
            try
            {
                packetInfo.DeserializeData(binaryUserData);
            }
            catch (Exception e)
            {
                //Send error packet
                return CreateErrorPacket();
            }
            //Process action according to the request
            return ChooseServerAction(packetInfo, binaryUserData);
        }


        //This class is used to init all server work
        public static void InitializeWork()
        {
            _cache = new CurrencyCache();
        }


        //This class is used to finish all server work
        public static void FinalizeWork()
        {
            try
            {
                _cache.Dispose();
            }
            catch (Exception e)
            {

            }
        }


        //Chooses action acording to the header packetInfo
        private static List<byte> ChooseServerAction(PacketInfo packetInfo, List<byte> userRequest)
        {
            switch ((PacketInfo.PacketType)packetInfo.Type)
            {
                case PacketInfo.PacketType.GET_RATES:   //User request currency rates
                    RequestCurrencyInfo requestCurrencies = new RequestCurrencyInfo();
                    try
                    {
                        requestCurrencies.DeserializeData(userRequest);
                        return RequireNewRates(requestCurrencies);
                    }
                    catch
                    {
                        return CreateErrorPacket();
                    }
            }
            return CreateErrorPacket();
        }


        //Create packet with requered rates
        private static List<byte> RequireNewRates(RequestCurrencyInfo requestCurrencyInfo)
        {
            if (requestCurrencyInfo.DateTo < requestCurrencyInfo.DateFrom)
            {
                return CreateErrorPacket();
            }
            //Get rates
            var rates = _cache.GetAvailableCurrencyRates((CurrencyType)requestCurrencyInfo.CurrencyTypeFrom, (CurrencyType)requestCurrencyInfo.CurrencyTypeTo,
                requestCurrencyInfo.DateFrom, requestCurrencyInfo.DateTo, 10000);
            //Create packet
            PacketInfo headerPacket = new PacketInfo(PacketInfo.PacketType.RESPONSE_RATES);
            ResponseRatesInfo responseRatesInfo = new ResponseRatesInfo();
            responseRatesInfo.RatesCount = rates.Count;
            List<CurrencyTransmittedInfo> ratesInfo = new List<CurrencyTransmittedInfo>(rates.Count);
            for (int i = 0; i < rates.Count; ++i)
            {
                var rate = rates[i];
                CurrencyTransmittedInfo rateInfo = new CurrencyTransmittedInfo();
                rateInfo.Date = rate.RateDate;
                rateInfo.Amount = rate.CurrencyAmountTo;
                ratesInfo.Add(rateInfo);
            }
            return NetworkPacketBuilder.CreateResponseRatesPacket(headerPacket, responseRatesInfo, ratesInfo.GetEnumerator());
        }


        private static List<byte> CreateErrorPacket()
        {
            List<byte> errorPacket = new List<byte>();
            PacketInfo packetInfo = new PacketInfo(PacketInfo.PacketType.ERROR);
            errorPacket.AddRange(packetInfo.SerializeData());
            return errorPacket;
        }


        private static CurrencyCache _cache;
    }
}
