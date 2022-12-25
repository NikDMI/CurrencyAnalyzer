using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigLibrary
{
    /// 
    /// Used by clients and servers to find out application settings\configs
    /// 
    public interface IConfig
    {
        //Describes network protocols, which can be used in the app
        public enum ApplicationProtocol { HTTP };

        public string GetServerDomainName();

        public int GetServerPort();

        public ApplicationProtocol GetNetworkProtocol();
    }
}
