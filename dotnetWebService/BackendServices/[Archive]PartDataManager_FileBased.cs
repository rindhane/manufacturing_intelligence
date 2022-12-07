using System.IO; //to access the FileStream
using System.Text; // to access the Encodingtypes
using System.Collections.Generic ; // to get access to List<T>
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PartDataManager_fileBased_archive{

    
    public record PartGenealogy{
        public string serial {get;set;} 

        public List<InspectionStats> dataPoints{get;set;} = new List<InspectionStats>();
    }
    public record InspectionStats{

        public string processId {get; set;}
        public string operater{get; set; }
        public string timeStamp{get; set; }
        public string machine{get; set; }
        public string characteristicStatus{get; set; }

        public string serialNum {get;set;}
    }
    public interface IpartDataHandler{
         public Task<List<PartGenealogy>> provideEntryList() ;

          public Task<PartGenealogy> searchPart(string serial); 

          public Task<string> returnJsonResult(string serial);
    }

public class PartDataHandler:IpartDataHandler{

    private string _filePath ;

    public PartDataHandler(string path)
    {
        _filePath=path;
        
    }

    async Task<string> readFile(){
        int buffer=4096;
        string result = string.Empty;
        FileStream fs= new FileStream(_filePath,
                                    FileMode.OpenOrCreate,
                                    FileAccess.Read,
                                    FileShare.ReadWrite,
                                    buffer, FileOptions.Asynchronous);
        var sr = new StreamReader(fs, Encoding.UTF8);//File.AppendText(_path);
        result= await sr.ReadToEndAsync();
        sr.Close();
        sr.Dispose();
        return result;
    }

    public async Task<List<PartGenealogy>> provideEntryList(){
        string temp = await readFile();
        List<PartGenealogy> result = new List<PartGenealogy>();
        result=JsonConvert.DeserializeObject<List<PartGenealogy>>(temp);
        return result;
    }
    
    public async Task<PartGenealogy> searchPart(string serial){
        PartGenealogy result =new PartGenealogy();
        List<PartGenealogy> temp = await provideEntryList();
        PartGenealogy ans = temp.Find(dat => dat.serial.ToLower().Equals(serial.ToLower()));// check does lower case is required
        if (ans==null) {
            return result;
        }
        return ans;
    }

    public async Task<string> returnJsonResult(string serial){
        var result = await searchPart(serial);
        return JsonConvert.SerializeObject(result);
    }

}
}