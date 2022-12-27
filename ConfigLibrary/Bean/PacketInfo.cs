using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    ///This struct is used in the begining of packets to indicate packet type
    public class PacketInfo : IConvertable
    {
        public enum PacketType: int { ERROR, GET_RATES, RESPONSE_RATES };

        public PacketInfo(PacketType packetType)
        {
            _packetType = packetType;
        }

        public int Type
        {
            get
            {
                return (int)_packetType;
            }

            set
            {
                _packetType = (PacketType)value;
            }
        }


        //Get serialized bytes of some structure
        public List<byte> SerializeData()
        {
            return BinaryConverter.SerializeData<PacketInfo>(this);
        }


        //Fill structure from serialized bytes
        public void DeserializeData(List<byte> data)
        {
            BinaryConverter.DeserializeData<PacketInfo>(data, this);
        }


        private PacketType _packetType;
    }
}
