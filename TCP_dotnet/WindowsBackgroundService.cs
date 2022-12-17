using Microsoft.Extensions.Hosting; // to get access to BackgroundService
using TCPSetup; // to get access to TCPrunner
using Microsoft.Extensions.Logging; //to get access to iLogger
using System.Threading.Tasks;
using System.Threading; //to get access to CancellationToken
using Microsoft.Extensions.Configuration;// to get access Iconfiguration

namespace WindowsBackgroundService  {

 public class ScannerBackgroundService : BackgroundService 
{
    private readonly string configFile ;
    private readonly ILogger<ScannerBackgroundService> _logger ;

    public ScannerBackgroundService(IConfiguration configuration,ILogger<ScannerBackgroundService> logger){
        configFile =  System.AppDomain.CurrentDomain.BaseDirectory //use another method to get abse directory in windowsservice                                         //System.Environment.GetEnvironmentVariable("directory")
                     + configuration["Configs:scannerConfigFile"];  //"scannerConfig.toml";
        System.Console.WriteLine(configFile);
        _logger=logger;
        //;
    }

    protected override async Task ExecuteAsync (CancellationToken stoppingToken){
        try {
            while(!stoppingToken.IsCancellationRequested){
                await TcpRunner.runnerFromConfig(configFile); 
            }
        }catch (System.Exception ex) {
            _logger.LogError(ex, "{Message}", ex.Message);
            System.Environment.Exit(1);
        }

    }
}

}