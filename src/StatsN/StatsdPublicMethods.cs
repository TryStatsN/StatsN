using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public partial class Statsd
    {
        public Task Count(string name, long count = 1) => LogMetric(name, count, Constants.Metrics.COUNT);
        public Task Gauge(string name, long value) => LogMetric(name, value, Constants.Metrics.GAUGE);
        public Task GaugeDelta(string name, long value) => LogMetric(name, value >= 0? $"+{value.ToString()}": value.ToString(), Constants.Metrics.GAUGE);
        public Task Timing(string name, long milliseconds) => LogMetric(name, milliseconds, Constants.Metrics.TIMING);
        public Task Timing(string name, Action actionToTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            actionToTime?.Invoke();
            stopwatch.Stop();
            return LogMetric(name, stopwatch.ElapsedMilliseconds, Constants.Metrics.TIMING);
        }
        public Task Set(string name, long value) => LogMetric(name, value, Constants.Metrics.SET);

    }
}
