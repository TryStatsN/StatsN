using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public interface IStatsd : IDisposable
    {
        /// <summary>
        /// Simple Counter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task CountAsync(string name, long count = 1);
        /// <summary>
        /// arbitrary values, which can be recorded.  
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task GaugeAsync(string name, long value);
        /// <summary>
        /// Add or remove from gauge instead of setting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task GaugeDeltaAsync(string name, long value);
        /// <summary>
        /// change the value of the gauge, rather than setting it
        /// </summary>
        /// <param name="name"></param>
        /// <param name="milliseconds">time to log in ms</param>
        /// <returns></returns>
        Task TimingAsync(string name, long milliseconds);
        /// <summary>
        /// How long something takes to complete. StatsD figures out percentiles, average (mean), standard deviation, sum, lower and upper bounds for the flush interval
        /// </summary>
        /// <param name="name"></param>
        /// <param name="actionToTime">action to instrument</param>
        /// <returns></returns>
        Task TimingAsync(string name, Action actionToTime);
        /// <summary>
        /// StatsD supports counting unique occurences of events between flushes, using a Set to store all occuring events
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetAsync(string name, long value);
    }
}
