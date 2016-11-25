using StatsN.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsN
{
    public partial class Statsd : IStatsd
    {
        readonly StatsdOptions options;

        readonly BaseCommunicationProvider _provider;

        public static Statsd New<T>(Action<StatsdOptions> configure) where T: BaseCommunicationProvider, new()
        {
            var options = new StatsdOptions();
            configure?.Invoke(options);
            return new Statsd(options, new T());
        }
        public static Statsd New<T>(StatsdOptions options) where T: BaseCommunicationProvider, new()
        {
            return new Statsd(options, new T());
        }
        public static Statsd New(StatsdOptions options) => Statsd.New<Udp>(options);
        public static Statsd New(Action<StatsdOptions> options) => Statsd.New<Udp>(options);
        public Statsd(StatsdOptions options, BaseCommunicationProvider provider)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if(provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (string.IsNullOrEmpty(options.HostOrIp))
            {
                throw new ArgumentNullException(nameof(options.HostOrIp));
            }
            if(options.Port < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Port));
            }
            this.options = options;
            _provider = provider.Construct(options);

        }
        public Task LogMetric(string metricName, long value, string metricType, string postfix = "")
        {
            if(value < 0)
            {
                options.LogEvent($"Metric {metricName} has a value of less than 0 which is invalid. Skipping", EventType.warning);
                return Task.FromResult(0);
            }
            return LogMetric(metricName, value.ToString(), metricType, postfix);
        }
        public async Task LogMetric(string metricName, string value, string metricType, string postfix = "")
        {
            if (!_provider.IsConnected && !await _provider.Connect().ConfigureAwait(false))
            {
                options.LogEvent("unable to connect message transport", EventType.error);
                return;
            }
            var calculateMetric = BuildMetric(metricName, value, metricType, options.Prefix, postfix);
            if (string.IsNullOrWhiteSpace(calculateMetric))
            {
                options.LogEvent($"Unable to generate metric for {metricType} value {value}", EventType.error);
            }
            try
            {
                await _provider.SendMetric(calculateMetric).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                options?.LogException(e);
            }
            
        }
        internal static string BuildMetric(string metricName, string value, string metricType, string prefix = "", string postfix = "")
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                Trace.TraceError("metric not passed to compile metric");
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                Trace.TraceError("value not passed to compile metric");
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(metricType))
            {
                Trace.TraceError("metric type not passed to compile metric. This really shouldnt happen");
                return string.Empty;
            }
            StringBuilder builder;
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                builder = new StringBuilder(prefix, prefix.Length + metricName.Length + 4 + metricType.Length + value.Length + postfix.Length);
                builder.Append(Constants.dot);
                builder.Append(metricName);
            }
            else
            {
                builder = new StringBuilder(metricName, metricName.Length + 3 + metricType.Length + value.Length + postfix.Length);
            }
            builder.Append(Constants.colon);
            builder.Append(value);
            builder.Append(Constants.pipe);
            builder.Append(metricType);
            if (!string.IsNullOrWhiteSpace(postfix))
            {
                builder.Append(Constants.pipe);
                builder.Append(postfix);
            }
            return builder.ToString();
        }
    }
}
