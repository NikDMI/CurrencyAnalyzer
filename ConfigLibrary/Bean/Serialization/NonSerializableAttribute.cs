using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary.Bean
{
    //this attr is used to prevent serialization of public feields/properties
    internal class NonSerializableAttribute : Attribute
    {
    }
}
