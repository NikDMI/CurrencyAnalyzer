using System;
using ConfigLibrary;

namespace ConfigLibrary.ConfigImpl
{
    /// 
    /// Represents configs that get informations from the assembly mememory sections
    ///
    internal class MemoryConfig : IConfig
    {

        public string GetServerDomainName()
        {
            return SERVER_DOMAIN_NAME;
        }


        public int GetServerPort()
        {
            return SERVER_PORT_NUMBER;
        }


        public IConfig.ApplicationProtocol GetNetworkProtocol()
        {
            return DEFAULT_NETWORK_PROTOCOL;
        }


        private const string SERVER_DOMAIN_NAME = "127.0.0.1";
        private const int SERVER_PORT_NUMBER = 6789;
        private const IConfig.ApplicationProtocol DEFAULT_NETWORK_PROTOCOL = IConfig.ApplicationProtocol.HTTP;
    }
}
