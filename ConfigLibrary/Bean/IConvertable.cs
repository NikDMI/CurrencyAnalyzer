using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    //Indicates that structure can be converted to/into special format
    public interface IConvertable
    {
        //Get serialized bytes of some structure
        public List<byte> SerializeData();


        //Fill structure from serialized bytes
        //After reading structure will be removed from the List
        public void DeserializeData(List<byte> data);
    }
}
