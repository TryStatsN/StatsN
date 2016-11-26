using StatsN.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public class StatsdOptions
    {
        public Action<System.Exception> OnExceptionGenerated;
        public Action<StatsdLogMessage> OnLogEventGenerated;

        public bool BufferMetrics { get; set; } = false;
        public int BufferSize { get; set; } = 512;
        public string HostOrIp { get; set; } = Constants.Localhost;
        public int Port { get; set; } = 8125;
        string prefix = string.Empty;
        public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                prefix = value.TrimEnd(Constants.dot);
            }
        }

        internal void LogException(System.Exception exception)
        {
            if(OnExceptionGenerated == null)
            {
                LogEvent(new StatsdLogMessage(exception.Message, EventType.Error));
                return;
            }
            OnExceptionGenerated.Invoke(exception);

            
        }
        internal void LogEvent(string message, EventType weight) => this.LogEvent(new StatsdLogMessage(message, weight));
        
        internal void LogEvent(StatsdLogMessage logMessage)
        {
            if(OnLogEventGenerated == null)
            {
                var traceMessage = $"{Constants.Statsd}: {logMessage.Message}";
                switch (logMessage.Weight)
                {
                    case EventType.Info:
                        Trace.TraceInformation(traceMessage);
                        break;
                    case EventType.Warning:
                        Trace.TraceWarning(traceMessage);
                        break;
                    case EventType.Error:
                        Trace.TraceError(traceMessage);
                        break;
                }
            }
            else
            {
                OnLogEventGenerated?.Invoke(logMessage);
            }
        }
    }
}
