using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace StatsN.UnitTests
{
    public class TcpTests
    {
        [Fact]
        public void NoConnectionShouldNotCauseExceptions()
        {
            var statsd = Statsd.New<Tcp>(new StatsdOptions()
            {
                HostOrIp = "127.0.0.1",
                Port = 8888
            });
            statsd.CountAsync("awesomemetric.yo");
            statsd.CountAsync("awesomemetric.yo");
            statsd.CountAsync("awesomemetric.yo");
            statsd.CountAsync("awesomemetric.yo");
        }
    }
}
