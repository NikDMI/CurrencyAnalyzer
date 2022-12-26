using System;
using Server.Servers;
using ConfigLibrary;

using Server.Cache;

namespace Server
{
    /// <summary>
    /// This class is used to start/stop server of the app
    /// </summary>
    public class ServerEntry
    {
        public static void Main(String[] args)
        {
            CurrencyCache c = new CurrencyCache();
            var res = c.GetAvailableCurrencyRates(ConfigLibrary.Bean.CurrencyType.BYN, ConfigLibrary.Bean.CurrencyType.USD, new DateTime(2022, 11, 1),
                new DateTime(2022, 11, 5), 10);
            res = c.GetAvailableCurrencyRates(ConfigLibrary.Bean.CurrencyType.BYN, ConfigLibrary.Bean.CurrencyType.USD, new DateTime(2022, 11, 10),
                new DateTime(2022, 11, 15), 10);
            res = c.GetAvailableCurrencyRates(ConfigLibrary.Bean.CurrencyType.BYN, ConfigLibrary.Bean.CurrencyType.USD, new DateTime(2022, 11, 1),
                new DateTime(2022, 11, 15), 10);

            //Get configs of the application
            IConfig configs = ConfigFactory.GetConfig(ConfigFactory.ConfigType.ASSEMBLY_MEMORY);
            try
            {
                //Create server object according to communication protocol
                _server = CreateServer(configs);
                //Bind server to the config address and start it
                string serverDomainName = configs.GetServerDomainName();
                int serverPort = configs.GetServerPort();
                _server.StartServer(serverDomainName, serverPort);
                Console.WriteLine("Server was started at " + serverDomainName + ":" + serverPort + "\nPress any key to stop the server...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't start the server. Error message: " + e.Message);
            }
            Console.Read();
            _server.StopServer();
            Console.WriteLine("Server was stopped");
        }


        private static IServer CreateServer(IConfig configs)
        {
            var communicationProtocol = configs.GetNetworkProtocol();
            switch (communicationProtocol)
            {
                case IConfig.ApplicationProtocol.HTTP:
                    return ServerFactory.CreateServer(ServerFactory.ServerKind.HTTP);

                default:
                    throw new Exception("Not supported comunication protocol");
            }
        }



        private static IServer _server;
    }
}
