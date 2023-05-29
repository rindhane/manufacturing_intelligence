using System.Collections.Generic ; // to get access to List<T>
using System.Threading.Tasks;
using Newtonsoft.Json;
using DbConnectors;
using System.Collections; // to access the DictionaryEntry type
using TOMLreader;
using DFQhandler; // to get access to dfq writer
using DFQhandler_new; //to access DFQWriter_v1;
using DFQJSON.helpers; // to access the JsonHelpers
using SPRLTransformers; // to acces ManualEntryJsonTransformer;
using DFQJSON.Models; // to acces DFQJSONModel


namespace PartDataManager{
    public record PartGenealogy{
        public string? serial {get;set;} 

        //public List<InspectionStats> dataPoints{get;set;} = new List<InspectionStats>();
        public List<IdataTypes> dataPoints{get;set;} = new List<IdataTypes>();
    }
    
    public interface IdataTypes{ //common interface to send the json packets of payload
        public string? processId {get;set;}
        public string? serialNum {get;set;}
    }

    public record JsonInput{ //common interface of query params as json from client
        public string? serial {get;set;}
        public int fileId {get;set;}
    }
    
    public record InspectionStats:IdataTypes{

        public string? processId {get; set;}
        public string? operater{get; set; }
        public string? timeStamp{get; set; }
        public string? machine{get; set; }
        public string? characteristicStatus{get; set; }

        public string? serialNum {get;set;}
        public string? opDesc {get;set;}
    }
    public record LabBlock:IdataTypes{

        public string? processId {get; set;} =  "LabResult";
        public string? labBlock{get; set; } 

        public string? serialNum {get;set;}
    }
    public interface IpartDataHandler{

        public Task<PartGenealogy> searchPart(string serial); 

        public Task<string> returnTraceJsonResult(string serial);

        public string provideConfiguredProdLines();
        public bool storePDF(string serialNum, string stationName, string pdfBase64);

        public string getPDFList(string serialNum);

        public Task<string> retrievePDF(string jsonString);
        public string getLabEntries();

        public string getAllTheProducitonLines();

        public string getAllTheOperationOfProducitonLine(string line);

        public void produceDfqForManualScan(string scanInput);

        public void produceDfqForManualForms(string scanInput);

        public string getAllPartCategory();
        public bool updateAllPartOperationFlow();
        
    }

    public class PartDataHandler:IpartDataHandler{

        private string? _configPath ;
        private QDasDbConnection? dbHandler;

        private tomlConfigReader? configReader;

        public PartDataHandler(string path)
        {
            _configPath=path;
            configReader = new tomlConfigReader(_configPath);
            var temp = buildOptionsfromConfig();
            dbHandler = new QDasDbConnection(temp.Item1,temp.Item2);
            updateAllPartOperationFlow();
        }
        public System.Tuple<dbOptions,string> buildOptionsfromConfig(){
            var source1Part = (string)configReader!.getKeyValue("dataSource","qdas_value_db");
            var dataSource =source1Part;
            try { // port variable were later added in the config file
                var source2Part = (string)configReader!.getKeyValue("port","qdas_value_db").ToString()!;
                dataSource = source1Part.Trim() + "," + source2Part.Trim();  
            }
            catch{
                System.Console.WriteLine("port details weren't provided");
            } 
            var opt = new dbOptions(){
                dataSource = dataSource,  
                userID = (string)configReader!.getKeyValue("userID","qdas_value_db"),            
                password = (string)configReader!.getKeyValue("password","qdas_value_db"),     
                dbName=(string)configReader!.getKeyValue("dbName","qdas_value_db")
            };
            var table = (string)configReader!.getKeyValue("ReportTableName","qdas_value_db");
            return System.Tuple.Create(opt,table);
        }
        
        public async Task<PartGenealogy> searchPart(string serial){
            PartGenealogy result =new PartGenealogy();
            var inputs= dbHandler!.fetchTraceabilitySearchDataOfPart(serial);
            result.serial=serial;
            foreach(DictionaryEntry item in inputs){
                var valArr = (string[])item.Value!;
                var stationEntry = new InspectionStats() {
                    processId = (string) item.Key,
                    operater= valArr[0],
                    timeStamp= valArr[1],
                    machine= valArr[2],
                    characteristicStatus = await getOperationStatus((string) item.Key ,serial,valArr[1]),
                    serialNum= valArr[3],
                    opDesc = valArr[4],
                };
                result.dataPoints.Add(stationEntry);
            }
            result.dataPoints.Add(getLabBlockForSeachPart(serial)); //addition of Lab report block
            return result;
        }

        public async Task<string> getOperationStatus(string opKey,string serial, string timeStamp){
            var partCode = dbHandler!.getPartTypeCode(serial);
            var operationId = dbHandler.getOperationId(partCode.PartTypeCode, partCode.LineName ,opKey);
            var num_characteristics = dbHandler.getAllCharacteriticsOfOperationCode(
                partCode.PartTypeCode,partCode.LineName,opKey);
            var items = dbHandler.getQueryinAlarmsSearch(operationId,serial);
            var count_check = System.Math.Min(items.Count,num_characteristics.Count);
            for (int i=0;i<count_check;i++){
                if(System.Int64.Parse(items[i][0].ToString()!)!=0){
                    return "not-ok";
                }
            }
            await Task.Delay(0);
            if(timeStamp==string.Empty){
                // if other values are not available then don't send the characteristicStatus
                return "";
            }
            return "ok";
        }

        public async Task<string>getOperationStatusEvaluation(string opKey,string serial, string timeStamp) {
            //correctionPending : complete the characteristics evaluation code
            var partCode = dbHandler!.getPartTypeCode(serial);
            var operationId = dbHandler.getOperationId(partCode.PartTypeCode, partCode.LineName ,opKey);
            var num_characteristics = dbHandler.getAllCharacteriticsOfOperationCode(
                partCode.PartTypeCode,partCode.LineName,opKey);
            var items = dbHandler.getQueryinAlarmsSearch(operationId,serial);
            var count_check = System.Math.Min(items.Count,num_characteristics.Count);
            for (int i=0;i<count_check;i++){
                if(System.Int64.Parse(items[i][0].ToString()!)!=0){
                    return "not-ok";
                }
            }
            await Task.Delay(0);
            if(timeStamp==string.Empty){
                // if other values are not available then don't send the characteristicStatus
                return "";
            }
            return "ok";
        }

        public LabBlock getLabBlockForSeachPart(string serial){
            var result = new LabBlock();
            if(IsPDFList(serial))
            {
                result.serialNum=serial;
                result.labBlock=serial;
            }
            return result;
        }
        
        public async Task<string> returnTraceJsonResult(string serial){
            var result = await searchPart(serial);
            return JsonConvert.SerializeObject(result);
        }

        public string provideConfiguredProdLines(){
            var result = configReader!.getArrayType<string>("lines","prodPlant");
            return JsonConvert.SerializeObject(result);
        }
        public bool storePDF(string serialNum, string stationName, string pdfBase64){
            if(dbHandler!.storePdfData(serialNum,stationName,pdfBase64)){
                return true;
            }
            return false;
        }

        public async Task<string> retrievePDF(string jsonString){
            //input example : {"serial":"1234567;07365;3;AG672;G04;P4567;07:09:26","fileId":"4"}
            string result= string.Empty;
            try {
                var temp = JsonConvert.DeserializeObject<JsonInput>(jsonString);
                List<object[]> datblock=await dbHandler!.getPDFData(temp!.serial!,temp.fileId);
                result = (string)datblock[0][0];
            }catch(System.Exception ex){
                System.Console.WriteLine($"PartDataManager:retrievePDF:Error:{ex.Message}");
            }
            return result;
        }

        public bool IsPDFList(string serialNum){            
            var result = dbHandler!.getPDFListForSerial(serialNum);
            if(result.Count>0){
                return true;
            }
            return false;
        }
        public string getPDFList(string serialNum){
            var result = dbHandler!.getPDFListForSerial(serialNum);
            return JsonConvert.SerializeObject(result);
        }

        public string getLabEntries(){
            var result = configReader!.getArrayType<string>("labs","prodPlant");
            return JsonConvert.SerializeObject(result);
        }
        public string getAllTheProducitonLines(){
            var result = dbHandler!.GetAllAvailableProductionLines();
            return JsonConvert.SerializeObject(result);
        }

        public string getAllTheOperationOfProducitonLine(string line){
            var result = dbHandler!.GetAllOperationOfProductionLine(line);
            return JsonConvert.SerializeObject(result);
        }

        public List<System.Tuple<string,string>> addElementToDfqObject(
            List<System.Tuple<string,string>> dfqContainer, string key,string value){
                dfqContainer.Add(
                    new System.Tuple<string,string>(
                        key,value
                    )
                );
                return dfqContainer;
        }

        public void produceDfqForManualScan(string scanInput){
            string directory=(string)configReader!.getKeyValue("folder_path","DFQ_FROM_SCAN");
            string fileName=System.DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() +".dfq";
            var writer = new DFQWriter(directory+fileName);
            var result = JsonConvert.DeserializeObject<Dictionary<string,string>>(scanInput);
            var dfqResult= new List<System.Tuple<string,string>>(); 
            var partDetail = new PartFlowCategoryInput();
            if(!result!.ContainsKey("partCode")){
                partDetail= dbHandler!.getPartTypeCode(result!["serialNum"]);
            }else{
                partDetail.PartTypeCode=result["partCode"];
                partDetail.LineName= result["LineNum"];
            }
            
            #region dfq Part data
            //prepare the inputs for the part data
            dfqResult= addElementToDfqObject(dfqResult,
                                                "K1001", partDetail.PartTypeCode ) ;// part code
            dfqResult= addElementToDfqObject(dfqResult,
                                                "K1086",
                                                result["OpStation"]
                                            ) ;//opcode
            dfqResult= addElementToDfqObject(dfqResult,
                                                "K1102",
                                                result["LineNum"]//partDetail.LineName //check which way is the appropriate way to define line name
                                            ) ;//LineName
            /* // removing he operation description not required
            dfqResult= addElementToDfqObject(dfqResult,
                                                "K1087",
                                    dbHandler!.getOperationDescriptionFromTEILId(
                                            dbHandler.getOperationId(partTypeCode,result["OpStation"]))
                                            ) ;//part description 
            */
            writer.writeHeader(1); //only one characteristic is being written
            writer.fileTextWriter(dfqResult,1); //write the part details 
            dfqResult.Clear();
            #endregion
            #region dfq Characteristics
            //prepare the characteristics data
            var characteristic_keys = new List<string>{
                                        "characteristic_id",// K2001
                                        "characteristic_desc", // K2002
                                        "characteristic_remark", //K2900
                                        "characteristic_UID", //K2997
                                        "characteristic_measurement_type", //K2142
                                    };
            var key_codes = new List<string>{
                "K2001",
                "K2002",
                "K2900",
                "K2997",
                "K2142"
            };
            var characteristic_keyAndValues=configReader.getSetOfTableValues("DFQ_FROM_SCAN",
                                    characteristic_keys);
            //creating dfqResult list of all the characteristic's Qdas type configuration
            for(int i=0;i<characteristic_keys.Count;i++){
                if(characteristic_keyAndValues.ContainsKey(characteristic_keys[i])){
                    dfqResult = addElementToDfqObject(dfqResult,
                                                    key_codes[i],
                                                    characteristic_keyAndValues[characteristic_keys[i]]
                    );
                }
            }
            dfqResult= addElementToDfqObject(dfqResult,"K2004",1.ToString());//all scan data are considered attributes
            writer.fileTextWriter(dfqResult,1); //write the characteritics config to dfq
            dfqResult.Clear();
            #endregion
            #region dfq measured data
            //prepare the measured data
            dfqResult= addElementToDfqObject(dfqResult,"K0020", 1000.ToString()); //it is necessary to provide subgroup*1000 for new data point
            dfqResult= addElementToDfqObject(dfqResult,"K0021", 0.ToString()); //all scan data are assumend to be ok
            dfqResult= addElementToDfqObject(dfqResult,"K0004",System.DateTime.Now.ToString("dd.MM.yyyyaHH:mm:ss").Replace('a','/')); //correctionPending, get the right format
            dfqResult= addElementToDfqObject(dfqResult,"K0008", 0.ToString()); //operatorID
            dfqResult= addElementToDfqObject(dfqResult,"K0010", 0.ToString()); //machine Id
            dfqResult= addElementToDfqObject(dfqResult,"K0055", result["serialNum"]);
            // get K-filed name for senderTag :
            try { 
                var Kfield_senderTag = (string)configReader!.getKeyValue("senderTag_KField","DFQ_FROM_SCAN");
                dfqResult = addElementToDfqObject(dfqResult,Kfield_senderTag, result["senderTag"]);
            }catch (System.Exception e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("senderTag_field was not written in the dfq ");
            }
            writer.fileTextWriter(dfqResult,1); //write the characteritics config to dfq
            dfqResult.Clear();            
            #endregion 
        }

        public void produceDfqForManualForms(string scanInput){
            string directory=(string)configReader!.getKeyValue("folder_path","DFQ_FROM_SCAN");
            string fileName=System.DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() +".dfq";
            var writer = new DFQWriter_v1(directory+fileName);
            var dfqResult = writer.getDFQTransformation<DFQJSONModel>(
                scanInput,
                ManualEntryJsonTransformer.validDFQJsonFromInputResponse,
                JsonHelpers.extractDFQResultFromStdJson
            );
            writer.DfqModeltoFileWrite(dfqResult);
        }
        public string getAllPartCategory() {
            var result=dbHandler!.GetAllAvailablePartCodes();
            return JsonConvert.SerializeObject(result);

        }

        public bool updateAllPartOperationFlow() {
            var result=dbHandler!.GetAllAvailablePartCodes();
            foreach(System.Tuple<string, string> detail in result){
                dbHandler.updatePartOperationFlow(detail.Item1);
            }
            return true;

        }
    }
}