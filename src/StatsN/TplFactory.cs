using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    /// <summary>
    /// Class that provides Task.FromResult functions for dotnet 4 and up
    /// </summary>
    public static class TplFactory
    {
        public static Task<Result> FromResult<Result>(Result result)
        {
#if net40
            return TaskEx.FromResult(result);
#else
            return Task.FromResult<Result>(result);
#endif
        }
        public static Task FromResult()
        {
#if net40
            return TaskEx.FromResult(0);
#else
            return Task.FromResult(0);
#endif
        }
    }

}
