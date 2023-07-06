using System.Collections.Generic; // to get access to Dictionary
using System.Text.RegularExpressions; // for regex
using System.Collections.Specialized; // to get access to OrderedDictionary
using System.Collections; // to get access to DictionaryEntry type
using System.Threading.Tasks;

namespace DbConnectors
{   
    public record PartFlowCategoryInput{ //this is the unique part and Line combination belongs for building part Flow 
        public string PartTypeCode {get;set;} = string.Empty;
        public string LineName {get;set;}  = string.Empty;
    }

    public class QDasDbConnection : DbConnection  {

        Hashtable PartOperationFlow = new Hashtable(); //stores opCodeToTEIL_Id,TEIL_IdToOpCode against PartCategory
        string? QDB_NAME {get;set;}
        string? reportTable {get;set;} 

        string serialNumCol {get;set;}= "WV0014";//Confirm the field is K0054/WV0054 and not K0014/WV0014 //

        public QDasDbConnection(dbOptions opt , string report_table) : base(opt)
        {
            QDB_NAME= opt.dbName;
            reportTable=report_table;//"LABREPORT" ;
        }
       
        public (OrderedDictionary, Dictionary<int,string>) getOperationFlowforPartCategory(PartFlowCategoryInput input){
            // provides a map of operationCode : unique TETEIL ID in sorted manner
            string query = 
                $"SELECT TEARBEITSGANG, TETEIL FROM [{QDB_NAME}].[dbo].[TEIL] " + 
                $"WHERE TETEILNR = '{input.PartTypeCode}' and TEWERKSTATT = '{input.LineName}' ";
                List<object[]> result = this.ValuesFromSQLquery(query,2);
                List<(int,int)>opRaw = new List<(int,int)>();
                for(int i=0;i<result.Count;i++) {
                    var temp = getOperationNum(result[i][0].ToString()!);
                    if (temp!=-1) {
                        try{ // try & catch to avoid the multiple entry operation code entries
                        opRaw.Add((temp,i));// adding all the operation and their index in result list
                        }catch(System.Exception e){
                            System.Console.WriteLine(e.Message);
                        } 
                    }
                }
                opRaw.Sort(); //sort the operation in ascending operation
                //prepare the map of opcode to unique ID and vice versa
                var opCodeToTEIL_Id= new OrderedDictionary();
                var TEIL_IdToOpCode = new Dictionary<int,string>();
                for (int i=0; i<opRaw.Count;i++){
                    opCodeToTEIL_Id.Add(result[opRaw[i].Item2][0].ToString()!,
                                        (int)result[opRaw[i].Item2][1]); // key is the original opcodename in TEARBEITSGANG 
                                                                        //and value is the ID in TETEIL 
                    TEIL_IdToOpCode.Add((int)result[opRaw[i].Item2][1], 
                                        result[opRaw[i].Item2][0].ToString()!); // vice-versa of the above mapping 
                }
                return (opCodeToTEIL_Id,TEIL_IdToOpCode);
        }
        
        public void updatePartOperationFlow(string PartCategory){
            // currently not storing the updated Part flow
            /*
            var result = getOperationFlowforPartCategory(PartCategory);
            if(PartOperationFlow.ContainsKey(PartCategory)){
                PartOperationFlow[PartCategory]=result;
                return ;
            }
            PartOperationFlow.Add(PartCategory,result);
            */ 
        }
        
        int getOperationNum(string operationString){//helper Function to parse opcode into int-type
            //this function gets 10 from OP10, OP-10, "OP 10" but not from "OP 1 0"
            string pattern = @"(?<opcode>\d+)" ;
            Regex rx = new Regex(pattern,
                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);            
            Match m = rx.Match(operationString);
            var temp= m.Groups["opcode"].Value;
            int result = -1;
            System.Int32.TryParse(temp, out result);
            return result;
        }
        public int getOperationId(string PartCategory, string LineName ,string operationString){
            /* // following commented out code is ghosted since not storing the part flow
            if(PartOperationFlow.ContainsKey(PartCategory)){
                var  temp =  (System.ValueTuple<OrderedDictionary, Dictionary<int,string>>) 
                PartOperationFlow[PartCategory]!; // receiving opCodeToTEIL_Id, TEIL_IdToOpCode from Hashtable
                var opCodeToTEIL_Id = temp.Item1;
                return (int) opCodeToTEIL_Id[operationString]!;
            }
            */
            // creating operationFlow for the part from the sql itself
            var result = getOperationFlowforPartCategory(new PartFlowCategoryInput {
                PartTypeCode = PartCategory,
                LineName=LineName
            });
            var opCodeToTEIL_Id = result.Item1;
            if(opCodeToTEIL_Id.Contains(operationString)){
                return (int) opCodeToTEIL_Id[operationString]!;
            } 
            return -1;
        }

        OrderedDictionary getAllOperationSequenceOfPartCategory(PartFlowCategoryInput part){
            /* // this code is ghosted since partflow is not being stored in the hashtable
            //it is assumed that the partCategory exists
            var  temp =  (System.ValueTuple<OrderedDictionary, Dictionary<int,string>>) 
                PartOperationFlow[PartCategory]!;
            */
            //it is assumed that the partCategory exists
            // creating operationFlow for the part from the sql itself
            var result = getOperationFlowforPartCategory(part);
            return result.Item1;
        }
        public object[] getCharacteristicSpecs(string PartCategory,string LineName ,string operationString, string characteristicCode){
            // gets the specs of the characteristics of specific characteristicCode 
            int operationID = getOperationId(PartCategory, LineName ,operationString);
            string query = 
            $"SELECT TOP(1) MEMERKBEZ, MENENNMAS, MEUGW, MEOGW FROM [{QDB_NAME}].[dbo].[MERKMAL] " + 
                $"WHERE MEMERKNR = '{characteristicCode}' AND METEIL = '{operationID}'";
            List<object[]> result = this.GetSingleRowFromSQLquery(query,4); 
            // result order is description, nominal, Upper tolerance, lower tolerance
            if (result.Count>0){
                return result[0];
            }
            return new object[0];
        }
        public List<object[]> getAllCharacteriticsOfOperationCode(string PartCategory, string LineName, string operationString){
            int operationID = getOperationId(PartCategory, LineName, operationString);
            string query = 
            $"SELECT MEMERKMAL, MEMERKNR FROM [{QDB_NAME}].[dbo].[MERKMAL] " +
            $"WHERE METEIL = {operationID}"; // return Characteristics ID and CharacteristicCode 
            List<object[]> result = this.ValuesFromSQLquery(query,2);
            return result; 
        }       
        
        /*
        public string getPartTypeCode(string serialNum){ //gets the partsTypeCode/TETEILNR/PartCategory from serialNum 
            string query = 
            $"SELECT TOP(1) WVTEIL FROM [{QDB_NAME}].[dbo].[WERTEVAR] " +
            $"WHERE {serialNumCol} = '{serialNum}' " + 
            $"ORDER BY WVDATZEIT DESC" ;  
            List<object[]> result = this.GetSingleRowFromSQLquery(query,1);
            if(result.Count>0){
                string tempId =result[0][0].ToString()!;
                query =  $"SELECT TOP(1) TETEILNR FROM [{QDB_NAME}].[dbo].[TEIL] " +
                         $"WHERE TETEIL = {tempId}";
                List<object[]> IdList = this.GetSingleRowFromSQLquery(query,1);
                return IdList[0][0].ToString()!;
            }
            return string.Empty;
        }
        */
        public PartFlowCategoryInput getPartTypeCode(string serialNum){ //gets the partsTypeCode/TETEILNR/PartCategory from serialNum 
            string query = 
            $"SELECT TOP(1) WVTEIL FROM [{QDB_NAME}].[dbo].[WERTEVAR] " +
            $"WHERE {serialNumCol} = '{serialNum}' " + 
            $"ORDER BY WVDATZEIT DESC" ;  
            List<object[]> result = this.GetSingleRowFromSQLquery(query,1);
            if(result.Count>0){
                string tempId =result[0][0].ToString()!;
                query =  $"SELECT TOP(1) TETEILNR, TEWERKSTATT FROM [{QDB_NAME}].[dbo].[TEIL] " +
                         $"WHERE TETEIL = {tempId}";
                List<object[]> IdList = this.GetSingleRowFromSQLquery(query,2);
                return new PartFlowCategoryInput { 
                    PartTypeCode=IdList[0][0].ToString()! ,
                    LineName= IdList[0][1].ToString()! 
                };
            }
            return new PartFlowCategoryInput ();
        }
        public OrderedDictionary fetchTraceabilitySearchDataOfPart(string serialNum){
           var partDetail = getPartTypeCode(serialNum); //this is the PartCategory code in TEIL table
           var result = new OrderedDictionary();
           if (partDetail.PartTypeCode!=string.Empty){
            var opSequenceDict=getAllOperationSequenceOfPartCategory(partDetail);
            var inspectionParamsInput = GetAllInspectionOperationParams(serialNum);
            foreach (DictionaryEntry ent in opSequenceDict){
                var checkKey = getOperationId(partDetail.PartTypeCode, partDetail.LineName ,(string)ent.Key);
                if(inspectionParamsInput.ContainsKey(checkKey)){
                    var input = inspectionParamsInput[checkKey]; 
                    result.Add((string)ent.Key, new string[]{
                                                (string)input[0], //WVPRUEFER : operator name
                                                (string)input[1], //WVDATZEIT : DATETIME 
                                                (string)input[2], //WVMASCHINE: Machine Name
                                                serialNum, // serial ID,
                                                input[3], //operationCode's Description 
                    });
                    continue;
                }
                result.Add((string)ent.Key, new string[]{
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    (string)ent.Key,
                    string.Empty,
                });
            }         
           }
           return result;
        }
        public Dictionary<int,string[]> GetAllInspectionOperationParams(string serialNum){
            string query =
            $"SELECT WVTEIL, WVMERKMAL, WVPRUEFER, WVDATZEIT, WVMASCHINE, WVWERT  FROM [{QDB_NAME}].[dbo].[WERTEVAR] " +
            $"WHERE {serialNumCol} = '{serialNum}' " + 
            $"ORDER BY WVDATZEIT DESC ;" ;  // to get the latest inpspection data first 
            List<object[]> input = this.ValuesFromSQLquery(query,6);
            var result = new Dictionary<int, string[]> (); //key : opID/WVTEIL; array order is [operator, TIMESTAMP, MachineName] 
            if (input.Count>0) {
                for(int i=0; i<input.Count;i++){
                    var key = (int)input[i][0] ;
                    if(result.ContainsKey(key)){
                        continue;
                    }
                    result.Add(key, new string[] 
                                    { getOperatorNameFromId((int)input[i][2]),//(string)input[i][2].ToString()!, //WVPRUEFER : operator name
                                      (string)input[i][3].ToString()!, //WVDATZEIT : DATETIME 
                                      getMachineNameFromId((int)input[i][4]),//(string)input[i][4].ToString()!, //WVMASCHINE: Machine Name
                                      getOperationDescriptionFromTEILId(key),
                                    }
                                );
                }
            }
            return result ;
        }

        public string getOperationDescriptionFromTEILId(int id){
            string query =
            $"SELECT TE_1087 FROM [{QDB_NAME}].[dbo].[TEIL] " +
            $"WHERE TETEIL = {id}" ;
            List<object[]> input = this.ValuesFromSQLquery(query,1);
            string result = string.Empty;
            if(input.Count>0){
                result=(string)input[0][0].ToString()!;
            }
            return result;
        }

        public string getOperatorNameFromId(int id){
            string query = 
            $"SELECT PRVORNAME FROM [{QDB_NAME}].[dbo].[PRUEFER] " +
            $"WHERE PRPRUEFER = {id}" ;
            List<object[]> input = this.ValuesFromSQLquery(query,1);
            string result = "None";
            if(input.Count>0){
                result=(string)input[0][0].ToString()!;
            }
            return result;
        }

        public string getMachineNameFromId(int id){
            string query =
             $"SELECT TEMASCHINEBEZ FROM [{QDB_NAME}].[dbo].[TEIL] " +
            $"WHERE TETEIL = {id}";
            //$"SELECT MABEZ FROM [{QDB_NAME}].[dbo].[MASCHINE] " +
            //$"WHERE MAMASCHINE = {id}" ;
            List<object[]> input = this.ValuesFromSQLquery(query,1);
            string result = "None";
            if(input.Count>0){
                result=(string)input[0][0].ToString()!;
            }
            return result;
        }        

        public List<object[]> getQueryinAlarmsSearch(int operationId, string serial){
            string query = $"SELECT DISTINCT ALARM_EW FROM [{QDB_NAME}].[dbo].[ALARM_VALUES] "+
                            $"WHERE K1000={operationId} AND K0055='{serial}' " + 
                            $"ORDER BY ALARM_DATETIME DESC" ;
            List<object[]> result = this.ValuesFromSQLquery(query,1);
            return result;
        }
        public bool storePdfData(string serialNum, string stationName, string pdfBase64){
            string query = $"INSERT INTO [{QDB_NAME}].[dbo].{reportTable} VALUES( '{serialNum}' , '{stationName}', '{pdfBase64}' ) ;";
            try {
            this.insert_new_row(query);
            return true;
            }catch (System.Exception ex){
                System.Console.WriteLine(ex.Message);
            }
            return false;
        }

        public List<object[]> getPDFListForSerial(string serialNum){
            string query = $"SELECT id, stationName FROM [{QDB_NAME}].[dbo].{reportTable} " +
                            $"WHERE serialNum='{serialNum}' ;";
            List<object[]> result = this.ValuesFromSQLquery(query,2);
            return result;
        }
        //new added delete pdf by pushparaj
        public List<object[]> deletePDFbyId(string id)
        {
            string query = $"delete FROM [{QDB_NAME}].[dbo].{reportTable} " +
                            $"WHERE id=@id ;";
            List<object[]> result = this.ValuesFromSQLquery(query, 2);
            return result;
        }


        public async Task<List<object[]>> getPDFData(string serialNum,int id){
            string query = $"SELECT pdfData FROM [{QDB_NAME}].[dbo].{reportTable} " +
                            $"WHERE id={id} AND serialNum='{serialNum}' ;";
            List<object[]> result = this.GetSingleRowFromSQLquery(query,1);
            await Task.Delay(0);
            return result;
        }

        public List<string> GetAllAvailableProductionLines(){
            string query =
            $"SELECT DISTINCT TEWERKSTATT FROM [{QDB_NAME}].[dbo].[TEIL] " +
            $"WHERE TEWERKSTATT != '{null}'" ; 
            List<object[]> input = this.ValuesFromSQLquery(query,1);
            var result = new List<string>(); // pushi in this, the lines found in qdas Teil table
            if (input.Count>0) {
                for(int i=0; i<input.Count;i++){
                    result.Add((string)input[i][0].ToString()!);
                }
            }
            return result ;
        }

        public List<string> GetAllOperationOfProductionLine(string line){// get all the lines defined in the qdas Teil table
            string query =
            $"SELECT DISTINCT TEARBEITSGANG FROM [{QDB_NAME}].[dbo].[TEIL] " +
            $"WHERE  TEWERKSTATT = '{line}'" ; 
            List<object[]> input = this.ValuesFromSQLquery(query,1);
            var result = new List<string>(); 
            if (input.Count>0) {
                for(int i=0; i<input.Count;i++){
                    result.Add((string)input[i][0].ToString()!);
                }
            }
            return result ;
        }

        public List<System.Tuple<string,string>> GetAllAvailablePartCodes(){
            string query =
            $"SELECT DISTINCT TETEILNR, TEBEZEICH FROM [{QDB_NAME}].[dbo].[TEIL] " +
            $"WHERE TETEILNR != '{null}'" ; 
            List<object[]> input = this.ValuesFromSQLquery(query,2);
            var result = new List<System.Tuple<string,string>>(); // push in this, the lines found in qdas Teil table
            if (input.Count>0) {
                for(int i=0; i<input.Count;i++){
                    result.Add(
                        new System.Tuple<string,string> ( 
                            (string)input[i][0].ToString()!,
                            (string)input[i][1].ToString()!
                        )
                    );
                }
            }
            return result ;
        }
    }
}

