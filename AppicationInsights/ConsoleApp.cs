using NLog;

namespace AppicationInsights
{
    internal class ConsoleApp
    {

        #region Fields
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public ConsoleApp()
        {

        }
        #endregion

        #region Public Methods

        public async Task Run()
        {
            var logEventInfo = new LogEventInfo(NLog.LogLevel.Trace, "", "NLog Trace Event");

            foreach (var property in logEventInfo.Properties)
            {
                logEventInfo.Properties[property.Key] = property.Value;
            }

            logEventInfo.Properties["XCV"] = "xcv";
            logEventInfo.Properties["AppAction"] = "LogEvent-Start";
            logEventInfo.Properties["BusinessProcessName"] = Constants.BusinessProcessName;
            logEventInfo.Properties["EnvironmentName"] = Constants.EnvironmentName;
            logEventInfo.Properties["InstanceName"] = Environment.MachineName;

            _logger.Trace(logEventInfo);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
