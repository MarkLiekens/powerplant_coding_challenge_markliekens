using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace powerplant_coding_challenge
{
    public static class LoggerManager
    {
        private static ILog _serviceLogger;
        public static ILog ServiceLogger
        {
            get
            {
                if (_serviceLogger == null) // Not thread safe! Add Sync lock if needed
                {
                    _serviceLogger = LogManager.GetLogger("powerplant-coding-challenge");
                }
                return _serviceLogger;
            }
        }
    }
}
