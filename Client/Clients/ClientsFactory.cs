using System;
using ConfigLibrary;

namespace Client.Clients
{
    internal class ClientsFactory
    {
        public static IClient GetClientFromAppConfig(IConfig config)
        {
            switch (config.GetNetworkProtocol())
            {
                case IConfig.ApplicationProtocol.HTTP:
                    return new HttpClient(config.GetServerDomainName(), config.GetServerPort());

                default:
                    throw new ArgumentException("Can't support this network protocol");
            }
        }

        private ClientsFactory() { }
    }
}
