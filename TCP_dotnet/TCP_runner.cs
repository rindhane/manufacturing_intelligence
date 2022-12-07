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
                try{
                    System.Console.WriteLine($"Connecting with the scanner at Address:{_scannerIP} on port:{_port}");
                    if(_client.Connect()){
                        System.Console.WriteLine("Connected with Scanner");
                        return ;
                    }
                }catch{
                    System.Console.WriteLine("Unable to connect with Scanner");
                    System.Console.WriteLine("Sleep for 2 secs");
                    System.Threading.Thread.Sleep(2000);
                };
            }    
        }

        public void keepContinuousWatch(){
         try {
            _client.readContinuousData();
            }catch{
                System.Console.WriteLine("Disconnected with the scanner");
            }
        }
    }
}