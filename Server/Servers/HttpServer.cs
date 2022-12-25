using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Server.Servers
{
    internal class HttpServer : IServer
    {

        /// 
        /// Starts the server according to config address
        /// 
        public void StartServer(string domainName, int portNumber)
        {
            _httpListener = new HttpListener();
            string serverUrl = "http://" + domainName + ":" + portNumber + "/";
            _httpListener.Prefixes.Add(serverUrl);
            _httpListener.Start();
            //Create server cylcle throught task
            _cancellationServerTokenSource = new CancellationTokenSource();
            _cancellationServerToken = _cancellationServerTokenSource.Token;
            _serverAwaitEvent = new AutoResetEvent(false);  //event signaled when client send message or server must be stopped
            _serverDispatcherTask = new Task(this.HandleServerMessages, _cancellationServerToken);
            _serverDispatcherTask.Start();
        }


        /// 
        /// Stops the server
        /// 
        public void StopServer()
        {
            _httpListener.Stop();
            _cancellationServerTokenSource.Cancel();    //Cancel dispatch cycle and assosiated task
            lock (_serverAwaitEvent)
            {
                _serverAwaitEvent.Set();    //signals server cycle to awake
            }
            _serverDispatcherTask.Wait();
        }


        /// 
        /// Start cycle of the server
        /// 
        private void HandleServerMessages()
        {
            while (true)
            {
                var httpContextTask = _httpListener.GetContextAsync();
                httpContextTask.ContinueWith(task => 
                {
                    lock (_serverAwaitEvent)
                    {
                        _serverAwaitEvent.Set();    //signals server thread
                    }
                });

                _serverAwaitEvent.WaitOne();    //Sleep until client doesn't send a requirest or admin can't stop the server
                //Admin stoped the server
                if (_cancellationServerToken.IsCancellationRequested)
                {
                    return;
                }
                //Get user requirest
                var httpContext = httpContextTask.Result;
                ProcessClientRequest(httpContext);
            }
        }


        /// 
        /// Process client requirest
        /// 
        /// <param name="httpContext"></param>
        private void ProcessClientRequest(HttpListenerContext httpContext)
        {

        }


        private HttpListener _httpListener;

        private Task _serverDispatcherTask;

        //private CancellationToken _cancellationServerToken;
        private CancellationTokenSource _cancellationServerTokenSource;
        private CancellationToken _cancellationServerToken;

        AutoResetEvent _serverAwaitEvent;   // event to monitor server activity

    }
}
