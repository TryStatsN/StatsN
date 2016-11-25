using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public interface IStatsd
    {
        Task LogMetric(string metricName, string value, string metricType, string postfix = "");
    }
}
