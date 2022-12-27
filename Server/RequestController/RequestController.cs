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
            List<byte> binaryResponse = new List<byte>();
            //Get packet info
            PacketInfo packetInfo = new PacketInfo();
            try
            {
                packetInfo.DeserializeData(binaryUserData);
            }
            catch (Exception e)
            {
                //Send error packet
                packetInfo.Type = (int)PacketInfo.PacketType.ERROR;
                binaryResponse.AddRange(packetInfo.SerializeData());
                return binaryResponse;
            }
            //Process action according to the request
            ChooseServerAction(packetInfo, binaryUserData);
            return binaryResponse;
        }


        //This class is used to finish all server work
        public static void FinalizeWork()
        {
            _cache.Dispose();
        }


        //Chooses action acording to the header packetInfo
        private static void ChooseServerAction(PacketInfo packetInfo, List<byte> userRequest)
        {
            switch (packetInfo.Type)
            {
                //case PacketInfo.PacketType.
            }
        }


        private static CurrencyCache _cache = new CurrencyCache();
    }
}
