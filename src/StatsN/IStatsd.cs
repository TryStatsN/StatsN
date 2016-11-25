using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public interface IStatsd
    {
        Task LogMetric(string metricName, string value, string metricType, string postfix = "");
        Task Count(string name, long count = 1);
        Task Gauge(string name, long value);
        Task GaugeDelta(string name, long value);
        Task Timing(string name, long milliseconds);
        Task Timing(string name, Action actionToTime);
        Task Set(string name, long value);
    }
}
