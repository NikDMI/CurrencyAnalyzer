using System;
using Server.Servers;

namespace Server
{
    /// <summary>
    /// This class is used to start/stop server of the app
    /// </summary>
    public class ServerEntry
    {
        public static void Main(String[] args)
        {
            //According to configs create server

            try
            {
                _server = ServerFactory.CreateServer(ServerFactory.ServerKind.HTTP);
                _server.StartServer("127.0.0.1", 1000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't start the server. Error message: " + e.Message);
            }
            Console.WriteLine("Server starting at ../ \n Press any key to stop the server...");
            Console.Read();

        }


        private static IServer _server;
    }
}
