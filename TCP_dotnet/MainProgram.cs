using TCPSetup;
using TOMLreader;

namespace MainProgram {
  public static class MainProgram
  {
    public static void Main()
    {
      var configReader=new tomlConfigReader("scannerConfig.toml");
      var scannerIP=(string)configReader.getKeyValue("ip","scanner_detail");
      var temp = (System.Int64)configReader.getKeyValue("port","scanner_detail");
      var port=System.Convert.ToInt32(temp);
      while(true) {
        try{
          var client= new TcpRunner(scannerIP,port);//new MyTcpClient(scannerIP, port);
          client.initiateConnection();
          client.keepContinuousWatch();
        }catch{
          System.Console.WriteLine("Sleep for 2 secs");
          System.Threading.Thread.Sleep(2000);
          System.Console.WriteLine("Will try to reconnect with the scanner");
        }
      }
    }
  }
}