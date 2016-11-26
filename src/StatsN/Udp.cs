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
        private UdpClient _udpClient = new UdpClient();
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
            await _udpClient.SendAsync(payload, payload.Length, endpoint).ConfigureAwait(false);
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
#if net45
            _udpClient.Close();
#else
            _udpClient.Dispose();
            _udpClient = null;
#endif
        }
    }
}
