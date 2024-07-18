using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AppicationInsights
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                //Change here enum for switching logging method:-
                SwitchLogging(LoggingType.ConsoleLog);

                var app = new ConsoleApp();
                app.Run().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                LogManager.Flush();
                LogManager.Shutdown();
                await Task.Delay(TimeSpan.FromMilliseconds(1000 * 30));
            }

        }

        #region ConfigureServices
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
            // build configuration
            var builder = new ConfigurationBuilder()
                .AddJsonFile("Configurations/appsettings.json", true, true)
                .AddJsonFile($"Configurations/appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            serviceCollection.AddSingleton((IConfiguration)configuration);
            serviceCollection.AddAutoMapper(typeof(Program));
            serviceCollection.AddSingleton<ConsoleApp>();
        }
        #endregion

        //Switch-Logging Method Start
        #region SwitchLogging Method
        private static void SwitchLogging(LoggingType loggingType)
        {
            var config = new LoggingConfiguration();

            switch (loggingType)
            {
                case LoggingType.ConsoleLog:
                    var logconsole = new ConsoleTarget("logconsole")
                    {
                        Layout = "${date:format=HH\\:mm\\:ss} ${logger} ${uppercase:${level}} ${message} ${exception:format=tostring} " +
                                  "XCV:${event-properties:item=XCV}, AppAction:${event-properties:item=AppAction}, " +
                                  "BusinessProcessName:${event-properties:item=BusinessProcessName}, EnvironmentName:${event-properties:item=EnvironmentName}, " +
                                  "InstanceName:${event-properties:item=InstanceName}"
                    };
                    config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
                    break;

                case LoggingType.File:
                    var logfile = new FileTarget("logfile")
                    {
                        FileName = "${basedir}/logs/logfile.txt",
                        Layout = "${longdate} ${uppercase:${level}} ${message} " +
                                  "XCV:${event-properties:item=XCV}, AppAction:${event-properties:item=AppAction}, " +
                                  "BusinessProcessName:${event-properties:item=BusinessProcessName}, EnvironmentName:${event-properties:item=EnvironmentName}, " +
                                  "InstanceName:${event-properties:item=InstanceName}"
                    };
                    config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
                    break;

                case LoggingType.ApplicationInsights:
                    var aiTarget = new ApplicationInsightsTarget()
                    {
                        Name = "aiTarget",
                        InstrumentationKey = "Your-Instrumentation-Key",
                        Layout = "${message}"
                    };
                    config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, aiTarget);
                    break;

                default:
                    // Handle invalid logging type
                    throw new ArgumentException("Invalid logging type specified.");
            }

            LogManager.Configuration = config;
        }

        #endregion
        //Switch-Logging Method End

    }
}
