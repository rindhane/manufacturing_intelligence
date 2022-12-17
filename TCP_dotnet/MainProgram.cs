using Microsoft.Extensions.Hosting; //to get access to Host.CreateDefaultBuilder & UseWindowService
using Microsoft.Extensions.Logging; //to get access to iLogger
using Microsoft.Extensions.Logging.EventLog; // to get access to EventLogSettings, EventLogLoggerProvider;
using Microsoft.Extensions.Logging.Configuration; // to get access LoggerProviderOptions;
using Microsoft.Extensions.Configuration; // to get access to AddJsonFile
using System.Threading.Tasks; //for access to async 
using WindowsBackgroundService; // to get access to ScannerBackgroundService 
using Microsoft.Extensions.DependencyInjection; // to get access to AddHostedService

namespace MainProgram {
  public static class MainProgram
  {
    public static void Main(string[] args)
    {
      var configFile = "appConfig.json";
      IHost host = Host.CreateDefaultBuilder(args)
      .ConfigureHostConfiguration(hostConfig =>{
        hostConfig.AddEnvironmentVariables();
        hostConfig.AddCommandLine(args);
      })
      .ConfigureAppConfiguration((hostingContext,config)=>{
                config.AddJsonFile(configFile, 
                                    optional:true,
                                    reloadOnChange:true);
      })
      .UseWindowsService(options =>
        {
            options.ServiceName = "QdasT_scanner";
        })
      .ConfigureServices(services =>
        {
          LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);
          services.AddHostedService<ScannerBackgroundService>();

        })
      .ConfigureLogging((context, logging) =>
      {
          // See: https://github.com/dotnet/runtime/issues/47303
          logging.AddConfiguration(
              context.Configuration.GetSection("Logging"));
          // check what's the other way to configur event logging or configure logging
          //var settings = new EventLogSettings();
          //settings.Filter
      })
      .Build();
      host.Run();
      //await host.RunAsync();
    }
  }

}