using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StatsN
{
    public class Tcp : BaseCommunicationProvider
    {
#pragma warning disable CC0052 // Make field readonly
#pragma warning disable CC0033 // Dispose Fields Properly
        private TcpClient Client = new TcpClient();
#pragma warning restore CC0033 // Dispose Fields Properly
#pragma warning restore CC0052 // Make field readonly
        private NetworkStream Stream;
        private IPEndPoint ipEndpoint;
#pragma warning disable CC0052 // Make field readonly
        private object padLock = new object();
#pragma warning restore CC0052 // Make field readonly
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
                    Client.ConnectAsync(this.ipEndpoint.Address, Options.Port).GetAwaiter().GetResult();
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
#if net45
            Client.Close();
#else
            Client.Dispose();
            Client = null;
#endif

        }

        public override async Task SendAsync(byte[] payload)
        {
            if(!Client.Connected || !Stream.CanWrite)
            {
                Options.LogEvent("Tcp Stream not ready to write bytes dropping payload on the floor", Exceptions.EventType.Warning);
                return;
            }
            try
            {
                await Stream.WriteAsync(payload, 0, payload.Length);
            }
            catch(Exception e)
            {
                this.Options.LogException(e);
            }
            
        }
    }
}
