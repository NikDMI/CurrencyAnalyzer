using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Servers
{
    /// <summary>
    /// Represents server abstraction
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Starts the server according to config address
        /// </summary>
        void StartServer(string domainName, int portNumber)
        {

        }


        /// <summary>
        /// Stops the server
        /// </summary>
        void StopServer()
        {

        }
    }
}
