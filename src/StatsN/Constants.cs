using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsN
{
    internal static class Constants
    {
        internal const char colon = ':';
        internal const char pipe = '|';
        internal const char dot = '.';
        internal const string Localhost = "127.0.0.1";
        internal const string StatsN = "StatsN";
        
        internal static class Metrics
        {
            /// <summary>
            /// The number of times something happened.
            /// </summary>
            public const string COUNT = "c";
            /// <summary>
            /// The time it took for something to happen.
            /// </summary>
            public const string TIMING = "ms";
            /// <summary>
            /// The value of some measurement at this very moment.
            /// </summary>
            public const string GAUGE = "g";
            /// <summary>
            /// The number of times each event has been seen.
            /// </summary>
            public const string SET = "s";
        }
    }
}
