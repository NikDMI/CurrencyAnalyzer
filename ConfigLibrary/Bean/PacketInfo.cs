using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    ///This struct is used in the begining of packets to indicate packet type
    public struct PacketInfo : IConvertable
    {
        public enum PacketType: int { ERROR };

        public int Type
        {
            get
            {
                return _packetType;
            }

            set
            {
                _packetType = value;
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
            BinaryConverter.DeserializeData<PacketInfo>(data, ref this);
        }


        private int _packetType;
    }
}
