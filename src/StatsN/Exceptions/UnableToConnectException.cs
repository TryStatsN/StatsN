using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatsN.Exceptions
{
    public class UnableToConnectException : Exception
    {
        public UnableToConnectException()
        {
        }

        public UnableToConnectException(string message) : base(message)
        {
        }

        public UnableToConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
