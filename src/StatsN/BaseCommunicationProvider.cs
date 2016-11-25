using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StatsN
{
    public abstract class BaseCommunicationProvider : IDisposable
    {
        protected StatsdOptions Options { get; private set; }
        internal BackgroundWorker worker { get; set; }
        private ConcurrentQueue<byte[]> Queue { get; } = new ConcurrentQueue<byte[]>();
        protected BaseCommunicationProvider()
        {
            //protoype code to buffer metrics
            worker = new BackgroundWorker();
            worker.DoWork += (a, b) =>
            {
                try
                {
                    var buffer = new List<byte>(Options.BufferSize);
                    byte[] bufferOut;
                    while (Queue.Count > 0)
                    {
                        if (!Queue.TryDequeue(out bufferOut)) continue;
                        if ((buffer.Count + Constants.newLine.Length + bufferOut.Length) < Options.BufferSize)
                        {
                            if(buffer.Count > 0)
                            {
                                buffer.AddRange(Constants.newLine);
                            }
                            buffer.AddRange(bufferOut);
                        }
                        else
                        {
                            var stringData = Encoding.UTF8.GetString(buffer.ToArray());
                            SendAsync(buffer.ToArray());
                            buffer.Clear();
                            buffer.AddRange(bufferOut);
                        }
                    }
                    if (buffer.Count > 0) SendAsync(buffer.ToArray());
                }
                catch(Exception e)
                {
                    Options.LogException(e);
                }
                
            };
        }
        internal BaseCommunicationProvider Construct(StatsdOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            this.Options = options;
            return this;
        }
        public abstract Task<bool> Connect();
        public abstract bool IsConnected { get; }

        internal Task SendMetric(string metric)
        {
            var payload = Encoding.UTF8.GetBytes(metric);
            if (Options.BufferMetrics)
            {
                Queue.Enqueue(payload);
                if (!worker.IsBusy) worker.RunWorkerAsync();
                return Task.FromResult(0);
            }
            return SendAsync(payload);
        }
        public abstract Task SendAsync(byte[] payload);

        protected Task<IPEndPoint> GetIpAddressAsync() => GetIpAddressAsync(this.Options.HostOrIp, this.Options.Port); 
        
        protected async Task<IPEndPoint> GetIpAddressAsync(string hostOrIPAddress, int port)
        {
            IPAddress ipAddress;
            // Is this an IP address already?
            if (!IPAddress.TryParse(hostOrIPAddress, out ipAddress))
            {
                try
                {
                    ipAddress = (await Dns.GetHostAddressesAsync(hostOrIPAddress).ConfigureAwait(false)).First(p => p.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (Exception)
                {
                    Options.LogEvent($"Failed to retrieve domain {hostOrIPAddress}", Exceptions.EventType.Error);
                    return null;
                }

            }
            return new IPEndPoint(ipAddress, port);
        }
        private bool disposedValue = false;
        public abstract void OnDispose();
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }
            if (disposing)
            {
                this.worker.Dispose();
                this.worker = null;
                OnDispose();
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
