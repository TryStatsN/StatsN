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

        public async override Task<bool> Connect() => true;

        public override void OnDispose(){}

        public async override Task SendAsync(byte[] payload){}
    }
}
