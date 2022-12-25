using System;
using ConfigLibrary;
using Client.Clients;

namespace Client
{
    internal class ViewModel
    {
        public ViewModel(IConfig appConfigs)
        {
            _client = ClientsFactory.GetClientFromAppConfig(appConfigs);
            _client.SendTest();
        }


        private IClient _client;
    }
}
