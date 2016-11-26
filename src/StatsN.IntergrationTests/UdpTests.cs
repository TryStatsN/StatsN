using StatsN.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StatsN.IntergrationTests
{
    public class UdpTests
    {
        [Fact]
        public void LogUdpStats()
        {
            var options = new StatsdOptions()
            {
                BufferMetrics = false,
                HostOrIp = "127.0.0.1",
                Port = 8125,
            };
            options.OnExceptionGenerated += Options_OnExceptionGenerated;
            options.OnLogEventGenerated += Options_OnLogEventGenerated;
            var statsd = Statsd.New<Udp>(options);

            var whenToStop = DateTime.Now.AddMinutes(5);
            var random = new Random();
            while(whenToStop > DateTime.Now)
            {
                var amount = random.Next(1, 6);
                for (int i = 0; i < amount; i++)
                {
                    statsd.CountAsync("autotest.counteryo").Wait();
                    statsd.GaugeAsync("autotest.gaugeyo", random.Next(50, 400)).Wait();
                    statsd.SetAsync("autotest.setyo", random.Next(100, 1000)).Wait();
                    statsd.TimingAsync("autotest.timeryo", random.Next(100, 1000)).Wait();

                }
                Thread.Sleep(300);

            }

        }
        [Fact]
        public void LogUdpStatsBuffered()
        {
            var options = new StatsdOptions
            {
                BufferMetrics = true,
                HostOrIp = "localhost",
                Port = 8125,
                OnExceptionGenerated = Options_OnExceptionGenerated,
                OnLogEventGenerated = Options_OnLogEventGenerated
            };
            var statsd = Statsd.New<Udp>(options);

            var whenToStop = DateTime.Now.AddMinutes(20);
            var random = new Random();
            while (whenToStop > DateTime.Now)
            {
                var amount = random.Next(1, 6);
                for (int i = 0; i < amount; i++)
                {
                    statsd.CountAsync("autotest.counteryo").Wait();
                    statsd.GaugeAsync("autotest.gaugeyo", random.Next(50, 400)).Wait();
                    statsd.SetAsync("autotest.setyo", random.Next(100, 1000)).Wait();
                    statsd.TimingAsync("autotest.timeryo", random.Next(100, 1000)).Wait();

                }
                Thread.Sleep(75);

            }
        }

        private void Options_OnLogEventGenerated(StatsdLogMessage msg)
        {
            
        }

        private void Options_OnExceptionGenerated(Exception e)
        {
            //throw e;
        }
    }
}
