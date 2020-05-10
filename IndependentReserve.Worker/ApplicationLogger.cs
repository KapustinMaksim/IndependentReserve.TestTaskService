using NLog;

namespace IndependentReserve.Worker
{
    public static class ApplicationLogger
    {
        public static Logger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
    }
}