using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StatsN
{
    public class Tcp : BaseCommunicationProvider
    {
        private TcpClient Client = new TcpClient();
        private NetworkStream Stream;
        private IPEndPoint ipEndpoint;
        private object padLock = new object();
        public override bool IsConnected
        {
            get
            {
                return Client.Connected;
            }
        }

        public override async Task<bool> Connect()
        {
            if(ipEndpoint == null)
            {
                ipEndpoint = await GetIpAddressAsync().ConfigureAwait(false);
            }
            if (ipEndpoint == null) return false;
            lock (padLock)
            {
                if (Client.Connected) return true; //this could change since things could q up
                try
                {
                    Client.ConnectAsync(ipEndpoint.Address, Options.Port).GetAwaiter().GetResult();
                    if (Client.Connected) Stream = Client.GetStream();
                    return Client.Connected && Stream != null && Stream.CanWrite;
                }
                catch (Exception e)
                {
                    this.Options.LogException(e);
                    return false;
                }
            }
            
        }

        public override void OnDispose()
        {
            Client.Dispose();
            Client = null;
        }

        public override Task SendAsync(byte[] payload)
        {
            if(!Client.Connected || !Stream.CanWrite)
            {
                Options.LogEvent("Tcp Stream not ready to write bytes dropping payload on the floor", Exceptions.EventType.Warning);
                return Task.FromResult(0);
            }
            return Stream.WriteAsync(payload, 0, payload.Length);
        }
    }
}
