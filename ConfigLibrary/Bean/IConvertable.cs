using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    //Indicates that structure can be converted to/into special format
    public interface IConvertable
    {
        //Get serialized bytes of some structure
        public byte[] SerializeData();


        //Fill structure from serialized bytes
        public void DeserializeData(byte[] data);
    }
}
