using System;


namespace Server.Servers
{
    /// <summary>
    /// Used to create different kinds of servers
    /// </summary>
    public class ServerFactory
    {

        //Kinds of available servers
        public enum ServerKind { HTTP };

        public static IServer CreateServer(ServerKind serverKind)
        {
            switch (serverKind)
            {
                case ServerKind.HTTP:
                    return null;

                default:
                    throw new ArgumentException("Required kind of server is not supported");
            }
        }

        private ServerFactory()
        {
        }
    }
}
