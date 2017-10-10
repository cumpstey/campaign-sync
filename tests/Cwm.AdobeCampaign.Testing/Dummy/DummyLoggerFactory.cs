using System;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.Testing.Dummy
{
    public class DummyLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string name)
        {
            return new DummyLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
