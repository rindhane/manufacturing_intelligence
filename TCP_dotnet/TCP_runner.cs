using TransporterSetups; // for access to ImessageTransporter
using System.Threading.Tasks;
using TOMLreader; //for access tomlConfigReader
using httpCommunication; // for access to SenderHttpClient

namespace TCPSetup{
    public class TcpRunner{
        string _scannerIP{get;set;}
        int _port{get;set;}
        MyTcpClient _client ; 
        public TcpRunner(string scannerIP, int port ){
            _scannerIP= scannerIP;//"127.0.0.1";
            _port = port;//6000;
            _client=new MyTcpClient(_scannerIP, _port);//var client=new MyTcpClient("192.168.10.1", 23);
        }

        public void initiateConnection(){
            while (true){ 
                System.Console.WriteLine($"Connecting with the scanner at Address:{_scannerIP} on port:{_port}");
                if(_client.Connect()){
                    System.Console.WriteLine("Connected with Scanner");
                    return ;
                }
                System.Console.WriteLine("Unable to connect with Scanner");
                System.Console.WriteLine("Sleep for 2 secs");
                System.Threading.Thread.Sleep(2000);
            }    
        }

        public void keepContinuousWatch(){// continuously read data and push to console
         try {
            _client.readContinuousData();
            }catch{
                System.Console.WriteLine("Disconnected with the scanner");
            }
        }
        public void keepContinuousPushToExternalClient(ImessageTransporter ExternalMessageClient){
            while (true) {
                string message = string.Empty; 
                try {
                    System.Console.WriteLine("checking if there is any data");
                    message = _client.receiveData().Trim();
                }catch {    
                    if(_client!.isChannelConnected()){
                        continue;
                    }else {
                        System.Console.WriteLine("Disconnected with the scanner");
                        return ;
                    }
                }
                if(!message.Equals(string.Empty))
                {
                    ExternalMessageClient.sendMessage(message);
                }    
            }
        }

        public static async Task<bool> runnerFromConfig(string configFile){
            var configReader=new tomlConfigReader(configFile);
            var webClient = new SenderHttpClient(configFile); 
            var scannerIP=(string)configReader.getKeyValue("ip","scanner_detail");
            var temp = (System.Int64)configReader.getKeyValue("port","scanner_detail");
            var port=System.Convert.ToInt32(temp);
            while(true) {
                try{
                var tcpClient= new TcpRunner(scannerIP,port);//new MyTcpClient(scannerIP, port);
                tcpClient.initiateConnection();
                //tcpClient.keepContinuousWatch(); //use it to test the capture on the command prompt 
                tcpClient.keepContinuousPushToExternalClient(webClient);
                }catch{
                System.Console.WriteLine("Sleep for 2 secs");
                System.Threading.Thread.Sleep(2000);
                System.Console.WriteLine("Will try to reconnect with the scanner");
                }
            }
            await Task.Delay(0);
            return false;
    }
}
}