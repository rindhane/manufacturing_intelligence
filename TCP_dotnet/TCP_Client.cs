using System;
using System.Net.Sockets;

namespace TCPSetup{
public class MyTcpClient
{
  //Reference Knowledge
  private TcpClient? _client ;
  public string _address;
  public Int32 _serverPort;

  private NetworkStream? _connectionStream;
  public MyTcpClient (string address, Int32 serverPort)  { 
    _address=address;
    _serverPort=serverPort;
  }
  public bool Connect()
    {
      try
      {
        // Create a TcpClient.
        // Note, for this client to work you need to have a TcpServer
        // connected to the same address as specified by the server, port
        // combination.
        // Prefer using declaration to ensure the instance is Disposed later.
        //using TcpClient client = new TcpClient(server, port);
        _client = new TcpClient(_address, _serverPort);
        // Get a client stream for reading and writing.
        _connectionStream = _client.GetStream();
        _connectionStream.ReadTimeout = 1000; // 3000 milliseconds is the readTimeout
        return true;
      }
      catch (ArgumentNullException e)
      {
        Console.WriteLine("ArgumentNullException: {0}", e.Message);
      }
      catch (SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e.Message);
      }
      catch (Exception e)
      {
        Console.WriteLine("Other UnhandledException:{0}", e.Message);
      }
      return false;
  }

  public bool Disconnect(){
    // Explicit close is not necessary since TcpClient.Dispose() will be
        // called automatically if using statement was used. 
        _connectionStream!.Close();
        _client!.Close();
        return true;
  }
  public bool sendData(string message){
     try 
     {
      // Translate the passed message into ASCII and store it as a Byte array.
      Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
      // Send the message to the connected TcpServer.
      _connectionStream!.Write(data, 0, data.Length);
      Console.WriteLine("Sent: {0}", message);
      return true;
     }catch (ArgumentNullException e)
      {
        Console.WriteLine("ArgumentNullException: {0}", e);
      }
      catch (SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e);
      }
      catch (Exception e)
      {
        Console.WriteLine("Other UnhandledException:{0}", e);
      }
     return false;
  }
  public string receiveData()
  {
    // Buffer to store the response bytes.
    var data = new Byte[4096]; 
    // String to store the response ASCII representation.
    String responseData = String.Empty;
    // Read the first batch of the TcpServer response bytes.
    Int32 bytes = _connectionStream!.Read(data, 0, data.Length);
    if (bytes > 0) {
      responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    }
    return responseData;
  }

  public bool isChannelConnected (){
    return _client!.Connected;
  }
  public void readContinuousData(){
    while(true){
      try{
          string message= receiveData();
          Console.WriteLine("checking if there is any data");
          if(!message.Equals(String.Empty))
          {
            Console.WriteLine("Received Data: {0}", message);
          }
      }catch{
        //Console.WriteLine(op.Message);
      }
      if(_client!.Connected){
          continue;
      }
      return ;
    }
  }
}

}