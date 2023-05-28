// See https://aka.ms/new-console-template for more information
using System.IO; //to get access to FileStream;
using System.Text; //to get accsss to Encoding.UTF8;
using System.Collections.Generic;
using System; // to use the Func Delegate
using DFQModelSet;

namespace DFQhandler{

    class DFQWriter_v1{

        string _filePath{get;set;}
        StreamWriter? _fileWriteHanlder {get;set;}
        public DFQWriter_v1 (string filePathString){
            _filePath=filePathString;
            checkForParentDirectory();
        }

        public void checkForParentDirectory(){
            if(!System.IO.Directory.Exists(_filePath)){
                var parentDir = System.IO.Directory.GetParent(_filePath);
                System.IO.Directory.CreateDirectory(parentDir!.FullName);
            }
        }

        public bool initiateFileHandler(){
            int buffer=4096;
            FileStream fs= new FileStream(_filePath,
                                        FileMode.Append,
                                        FileAccess.Write,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            _fileWriteHanlder=sw;
            return true;
            
        }
        public bool closeFileHandler(){
            _fileWriteHanlder!.Close();
            _fileWriteHanlder.Dispose();
            return true;
        }
        
        public bool fileTextWriter(List<Tuple<string,string>> writeInput, int position, bool ISheader=false ) {
            
            foreach (Tuple<string,string> vals in writeInput ){
                if(!ISheader) {
                    _fileWriteHanlder!.WriteLine("{0}/{1} {2}", vals.Item1, position.ToString() ,vals.Item2); 
                    continue;   
                }
                _fileWriteHanlder!.WriteLine("{0} {1}", vals.Item1 ,vals.Item2);
            }
            
            return true;
        }

        public void writeHeader(string characteristic_count){
            var temp = new List<System.Tuple<string,string>>{
                new System.Tuple<string,string>("K0100", characteristic_count.ToString())
            };
            fileTextWriter(temp,1,true);
        }

        public void setDataTransformer<T>(Func<string,T> transformer, string dataString){
            var result = transformer(dataString);
        }

        public void DfqModeltoFileWrite(DFQmodel model){
            initiateFileHandler();
            writeHeader(model.K0100!);
            if(model.partData is not null){
                for(int i =0; i<model.partData!.Count;i++){
                    foreach(var partDetailKey in typeof(DFQPartModel).GetProperties()){ //looping over all possible keys 
                        //writing all the part related DFQ keys
                        if(partDetailKey.GetValue(model.partData[i])!=null // not writing the null keys    
                        & partDetailKey.PropertyType!=typeof(List<DFQCharacteristicModel>)) // not including chars in loop
                        {
                             _fileWriteHanlder!.WriteLine("{0}/{1} {2}", 
                            partDetailKey.Name, 
                            i+1, //position number of part , one-indexed
                            partDetailKey.GetValue(model.partData[i])!.ToString()!);  
                        }
                        //finished writing part data
                        
                    }
                    //writing all chars details
                    if (model.partData[i].chars is not null)
                    {
                        for (int j = 0 ; j<model.partData[i].chars!.Count;j++)
                        {
                            //writing char specs
                            foreach(var charSpecKey in typeof(DFQCharacteristicModel).GetProperties()) {
                                if(charSpecKey.GetValue(model.partData[i].chars![j])!=null // not writing the null keys    
                                & charSpecKey.PropertyType!=typeof(List<DFQMeasurementModel>))
                                {
                                    _fileWriteHanlder!.WriteLine("{0}/{1} {2}",
                                    charSpecKey.Name, //Kfield Name
                                    j+1, //position number of characterisitic, one-indexed
                                    charSpecKey.GetValue(model.partData[i].chars![j])!.ToString() // value obtained for characteristic name
                                    );
                                }
                            }
                            //finished writing char specs
                            //writing measure values
                            if (model.partData[i].chars![j].measurementVals is not null){
                                for (int k =0 ; k<model.partData[i].chars![j].measurementVals!.Count;k++){
                                    //writing measurementModel specs
                                    foreach(var measurementKey in typeof(DFQMeasurementModel).GetProperties()){
                                        if(measurementKey.GetValue(model.partData[i].chars![j].measurementVals![k])!=null //not writing the null keys 
                                        ){
                                            _fileWriteHanlder!.WriteLine("{0}/{1}/{2} {3}", 
                                                measurementKey.Name,
                                                j+1,// position number of related characteristic , one-indexed
                                                k+1, // position number of measurement data, one-indexed
                                                measurementKey.GetValue(model.partData[i].chars![j].measurementVals![k])!.ToString() // value obtained for measurement name 
                                            );
                                        }
                                    }
                                }
                            }
                        }    
                    }
                }
            }
            closeFileHandler();
        } 
         
    }
}
