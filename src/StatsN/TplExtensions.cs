using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{

    public static class TplFactory
    {
        public static Task<Result> FromResult<Result>(Result result)
        {
#if net40
            var taskSource = new TaskCompletionSource<Result>();
            taskSource.SetResult(result);
            return taskSource.Task;
#else
            return Task.FromResult<Result>(result);
#endif
        }
        public static Task FromResult()
        {
#if net40
            //gross but .net 4
            return System.Threading.Tasks.Task.Factory.StartNew(() => { });
#else
            return Task.FromResult(0);
#endif
        }
    }

}
