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
        /// <summary>
        /// User Options
        /// </summary>
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
                        if ((buffer.Count + bufferOut.Length) < Options.BufferSize)
                        {
                            buffer.AddRange(bufferOut);
                        }
                        else
                        {
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
        /// <summary>
        /// Connect the socket
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> Connect();
        /// <summary>
        /// Is the socket connected?
        /// </summary>
        public abstract bool IsConnected { get; }
        /// <summary>
        /// Send metric to inherited provider
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        internal Task SendMetric(string metric)
        {
            var payload = Encoding.ASCII.GetBytes(metric + Environment.NewLine);
            if (Options.BufferMetrics)
            {
                Queue.Enqueue(payload);
                if (!worker.IsBusy) worker.RunWorkerAsync();
                return TplFactory.FromResult();
            }
            return SendAsync(payload);
        }
        public abstract Task SendAsync(byte[] payload);
        /// <summary>
        /// Get the IPEndpoint for the Options object, will return null if it cannot be established
        /// </summary>
        /// <returns></returns>
        protected Task<IPEndPoint> GetIpAddressAsync() => GetIpAddressAsync(this.Options.HostOrIp, this.Options.Port);
        /// <summary>
        /// Get the IPEndpoint for the Options object, will return null if it cannot be established
        /// </summary>
        /// <param name="hostOrIPAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
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
