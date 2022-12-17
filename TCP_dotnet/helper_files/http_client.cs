using TOMLreader;
using System.Net.Http; //for access to HttpClient
using System.Net.Http.Json; // for access to JsonContent.Create
using System.Threading.Tasks; 
using System.Text.Json; // for access to  JsonSerializerOptions
using TransporterSetups; // for access to ImessageTransporter;

namespace httpCommunication {

public record dataBlock {

    public string? serialNum {get;set;}
    public string? LineNum {get;set; }
    public string? OpStation {get;set; }
    public string? senderTag {get;set;}
}

public class SenderHttpClient : ImessageTransporter {

    HttpClient _client;
    tomlConfigReader configurationProvider;
    System.Uri url_path ; 

    public SenderHttpClient (string configPath){
        _client = new HttpClient();
        configurationProvider = new tomlConfigReader(configPath);
        var builder = new System.UriBuilder ();
        builder.Host = (string)configurationProvider!.getKeyValue("ip_address_server","QdasT_config").ToString()!;
        builder.Port = System.Int32.Parse(configurationProvider!.getKeyValue("port_server","QdasT_config").ToString()!);
        builder.Scheme = "http";
        builder.Path = (string)configurationProvider!.getKeyValue("endpoint","QdasT_config").ToString()!;
        url_path = builder.Uri;
    }

    public dataBlock preparePayload(string serial){
        var result = new dataBlock();
        result.LineNum = (string)configurationProvider!.getKeyValue("LineNum","scanner_detail").ToString()!;
        result.OpStation = (string)configurationProvider!.getKeyValue("OpStation","scanner_detail").ToString()!;
        result.senderTag = (string)configurationProvider!.getKeyValue("senderTag","scanner_detail").ToString()!;
        result.serialNum = serial;
        return result;
    }

    public async Task<bool> sendSerialNum(string serial){
        var payloadData = preparePayload(serial);
        var JsonOptions = new JsonSerializerOptions();
        JsonOptions.PropertyNameCaseInsensitive = false;
        var payload = JsonContent.Create(payloadData,typeof(dataBlock), options :JsonOptions);
        try {
            var response = await _client.PostAsync(url_path,payload);
            if (!response.StatusCode.Equals(System.Net.HttpStatusCode.OK)){
                System.Console.WriteLine("data was not accepted at server");
            }
        }catch (System.Exception ex){
            System.Console.WriteLine(ex.Message);
            return false;
        }
        return true;
    }
    public async Task sendMessage(string Message){
        var result = await sendSerialNum(Message);
        if(!result) {
            System.Console.WriteLine($"message: {Message} was not sent to the server");
        }        
    }
}

}