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
            return Task.FromResult<Result>(result);

        }
        public static Task FromResult()
        {         
            return Task.FromResult(0);
        }
    }

}
