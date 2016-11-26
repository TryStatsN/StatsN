using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN
{
    public class NullChannel : BaseCommunicationProvider
    {
        public override bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public override Task<bool> Connect()
        {
            return Task.FromResult(true);
        }

        public override void OnDispose(){}

        public override Task SendAsync(byte[] payload)
        {
            return Task.FromResult(0);
        }
    }
}
