using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StatsN
{
    public class Udp : BaseCommunicationProvider
    {
#pragma warning disable CC0052 // Make field readonly
#pragma warning disable CC0033 // Dispose Fields Properly
        private UdpClient _udpClient = new UdpClient();
#pragma warning restore CC0033 // Dispose Fields Properly
#pragma warning restore CC0052 // Make field readonly
        private IPEndPoint _ipEndpoint;

        public override async Task SendAsync(byte[] payload)
        {
            IPEndPoint endpoint;
            if(_ipEndpoint == null)
            {
                endpoint = await GetIpAddressAsync().ConfigureAwait(false);

                if (endpoint == null) return;
            }
            else
            {
                endpoint = _ipEndpoint;
            }
            try
            {
#if net40
                await TaskEx.Run(() => _udpClient.Send(payload, payload.Length, endpoint)).ConfigureAwait(false);
#else
                await _udpClient.SendAsync(payload, payload.Length, endpoint).ConfigureAwait(false);
#endif
            }
            catch(Exception e)
            {
                Options.LogException(e);
            }

        }
        public override bool IsConnected
        {
            get
            {
                return _ipEndpoint != null;
            }
        }
        public override async Task<bool> Connect()
        {
            if (string.IsNullOrWhiteSpace(Options.HostOrIp))
            {
                Trace.TraceError($"{nameof(Options.HostOrIp)} not passed to statsd udp client");
            }
            if (Options.Port < 1)
            {
                Trace.TraceError($"{nameof(Options.Port)} not passed to statsd udp client");
            }
            _ipEndpoint = await GetIpAddressAsync(Options.HostOrIp, Options.Port).ConfigureAwait(false);
            return _ipEndpoint != null;
        }
        public override void OnDispose()
        {
#if NETFULL
            _udpClient.Close();
#else
            _udpClient.Dispose();
            _udpClient = null;
#endif
        }
    }
}
